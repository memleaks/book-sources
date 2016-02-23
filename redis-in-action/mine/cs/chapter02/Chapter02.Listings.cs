using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using System.Threading;
using helper;
using System.Web;

namespace RedisInAction.CH02
{
	internal class Inventory
	{
		internal string ID { get; private set; }
		Inventory(string id) {
			ID = id;
		}

		internal static Inventory Get(string id) {
			return new Inventory(id);
		}

		internal string ToJson(long timestamp) {
			var format =
@"{{
  'id'    : {0},
  'data'  : 'data to cache...',
  'cached': {1}
}}";
			return string.Format(format, ID, timestamp);
		}
	}

	partial class Chapter02
	{
		private const int DEFAULT_LIMIT = 10000000;
		private int LIMIT = DEFAULT_LIMIT;
		private bool QUIT = false;

		private string CheckToken(IDatabase conn, string token) {
			return conn.HashGet("login:", token);
		}

		private void UpdateToken(IDatabase conn, string token, string user, string item = null) {
			var timestamp = Now();
			conn.HashSet("login:", token, user);
			conn.SortedSetAdd("recent:", token, timestamp);
			if (!string.IsNullOrEmpty(item)) {
				conn.SortedSetAdd("viewed:" + token, item, timestamp);
				conn.SortedSetRemoveRangeByRank("viewed:" + token, 0, -26);
				conn.SortedSetIncrement("viewed:", item, -1);
			}
		}

		private void CleanSessions(IDatabase conn) {
			while (!QUIT) {
				var size = (int)conn.SortedSetLength("recent:");
				if (size <= LIMIT) {
					Thread.Sleep(1.Seconds());
					continue;
				}

				var end_index = Math.Min(size - LIMIT, 100);
				var tokens = conn.SortedSetRangeByRank("recent:", 0, end_index - 1);

				var session_keys = new List<RedisKey>();
				foreach (var token in tokens) {
					session_keys.Add("viewed:" + token);
				}

				conn.KeyDelete(session_keys.ToArray());
				conn.HashDelete("login:", tokens);
				conn.SortedSetRemove("recent:", tokens);
			}
		}

		private void CleanFullSessions(IDatabase conn) {
			while (!QUIT) {
				var size = (int)conn.SortedSetLength("recent:");
				if (size <= LIMIT) {
					Thread.Sleep(1.Seconds());
					continue;
				}

				var end_index = Math.Min(size - LIMIT, 100);
				var sessions = conn.SortedSetRangeByRank("recent:", 0, end_index - 1);

				var session_keys = new List<RedisKey>();
				foreach (var sess in sessions) {
					session_keys.Add("viewed:" + sess);
					session_keys.Add("cart:" + sess);
				}

				conn.KeyDelete(session_keys.ToArray());
				conn.HashDelete("login:", sessions);
				conn.SortedSetRemove("recent:", sessions);
			}
		}

		private void AddToCart(IDatabase conn, string session, string item, int count) {
			if (count <= 0) {
				conn.HashDelete("cart:" + session, item);
			}
			else {
				conn.HashSet("cart:" + session, item, count);
			}
		}

		private bool CanCache(IDatabase conn, string request) {
			var item_id = ExtractItemID(request);
			if (string.IsNullOrEmpty(item_id) || IsDynamic(request)) {
				return false;
			}
			var rank = conn.SortedSetRank("viewed:", item_id);
			return (rank ?? 0) < 10000;
		}

		private string ExtractItemID(string request) {
			var parsed = new Uri(request);
			var query = HttpUtility.ParseQueryString(parsed.Query);
			return query["item"];
		}

		private bool IsDynamic(string request) {
			var parsed = new Uri(request);
			var query = HttpUtility.ParseQueryString(parsed.Query);
			return query.AllKeys.Contains("_");
		}

		private string HashRequest(string request) {
			return request.GetHashCode().ToString();
		}

		private string CacheRequest(IDatabase conn, string request, Func<string, string> callback) {
			if (!CanCache(conn, request)) {
				return "generated " + callback(request);
			}

			var page_key = "cache:" + HashRequest(request);
			var content = conn.StringGet(page_key);
			if (string.IsNullOrEmpty(content)) {
				content = "cached " + callback(request);
				conn.StringSet(page_key, content, 300.Seconds());
			}
			return content;
		}

		private void RescaleViewed(IDatabase conn) {
			while (!QUIT) {
				conn.SortedSetRemoveRangeByRank("viewed:", 0, -20001);
				conn.SortedSetCombineAndStore(SetOperation.Intersect,
					"viewed:", new RedisKey[] { "viewed:" }, new double[] { 0.5 });
				Thread.Sleep(300.Seconds());
			}
		}

		private void ScheduleRowCache(IDatabase conn, string row_id, int delay) {
			conn.SortedSetAdd("delay:", row_id, delay);
			conn.SortedSetAdd("schedule:", row_id, Now());
		}

		private void CacheRows(IDatabase conn) {
			while (!QUIT) {
				var next = conn.SortedSetRangeByRankWithScores("schedule:", 0, 0);
				var now = Now();
				if (next.IsNullOrEmpty() || next[0].Score > now) {
					Thread.Sleep(0.05.Seconds());
					continue;
				}

				var row_id = next[0].Element;
				var delay = conn.SortedSetScore("delay:", row_id);
				if (delay <= 0) {
					conn.SortedSetRemove("delay:", row_id);
					conn.SortedSetRemove("schedule:", row_id);
					conn.KeyDelete("inv:" + row_id);
					continue;
				}

				var row = Inventory.Get(row_id);
				conn.SortedSetAdd("schedule:", row_id, now + delay.Value);
				conn.StringSet("inv:" + row_id, row.ToJson(Now()));
			}
		}
	}
}
