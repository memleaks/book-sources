using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using helper;
using StackExchange.Redis;

namespace RedisInAction.CH01
{
	partial class Chapter01
	{
		private const long ONE_WEEK_IN_SECONDS = 7 * 86400;
		private const int VOTE_SCORE = 432;
		private const int ARTICLES_PER_PAGE = 25;

		private long PostArticle(IDatabase conn, string user, string title, string link) {
			var article_id = conn.StringIncrement("article:");
			var voted = "voted:" + article_id;
			conn.SetAdd(voted, user);
			conn.KeyExpire(voted, ONE_WEEK_IN_SECONDS.Seconds());

			var now = Now();
			var article = "article:" + article_id;
			conn.HashSet(article, new[] {
				new HashEntry("title", title),
				new HashEntry("link", link),
				new HashEntry("poster", user),
				new HashEntry("time", now),
				new HashEntry("votes", 1),
			});

			conn.SortedSetAdd("score:", article, now + VOTE_SCORE);
			conn.SortedSetAdd("time:", article, now);
			return article_id;
		}

		private void VoteArticle(IDatabase conn, string user, string article) {
			long cutoff = Now() - ONE_WEEK_IN_SECONDS;
			if ((conn.SortedSetScore("time:", article) ?? long.MinValue) < cutoff) {
				return;
			}

			var article_id = article.Split(':').Last();
			if (conn.SetAdd("voted:" + article_id, user)) {
				conn.SortedSetIncrement("score:", article, VOTE_SCORE);
				conn.HashIncrement(article, "votes", 1);
			}
		}

		private List<RedisHash> GetArticles(IDatabase conn, int page, string order = "score:") {
			var start = (page - 1) * ARTICLES_PER_PAGE;
			var end = start + ARTICLES_PER_PAGE - 1;
			var ids = conn.SortedSetRangeByRank(order, start, end, Order.Descending);

			var articles = new List<RedisHash>();
			foreach (var id in ids) {
				var data = conn.HashGetAll(id.ToString());
				var article_data = new RedisHash(data);

				article_data["id"] = id;
				articles.Add(article_data);
			}
			return articles;
		}

		private void AddRemoveGroup(IDatabase conn, long article_id, string[] to_add = null, string[] to_remove = null) {
			var article = "article:" + article_id;
			if (to_add != null) {
				foreach (var group in to_add) {
					conn.SetAdd("group:" + group, article);
				}
			}
			if (to_remove != null) {
				foreach (var group in to_remove) {
					conn.SetRemove("group:" + group, article);
				}
			}
		}

		private List<RedisHash> GetGroupArticles(IDatabase conn, string group, int page, string order = "score:") {
			var key = order + group;
			if (!conn.KeyExists(key)) {
				conn.SortedSetCombineAndStore(SetOperation.Intersect, key, "group:" + group, order, Aggregate.Max);
				conn.KeyExpire(key, 60.Seconds());
			}
			return GetArticles(conn, page, key);
		}
	}
}
