using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using helper;
using NUnit.Framework;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Threading;

namespace RedisInAction.CH02
{
	partial class Chapter02 : Chapter
	{
		[TearDown]
		public void TearDown() {
			QUIT = false;
			LIMIT = DEFAULT_LIMIT;
		}

		[RIATest]
		public void TestLoginCookies() {
			var token = Guid.NewGuid().ToString();
			var user = "username";
			UpdateToken(connection, token, user, "itemX");
			PrintLine("We just logged-in/updated token: {0}", token);
			PrintLine("For user: {0}", user);

			PrintLine("What username do we get when we look-up that token?");
			var r = CheckToken(connection, token);
			Assert.That(r, Is.Not.Null.Or.Empty);
			PrintLine(r);

			PrintLine("Let's drop the maximum number of cookies to 0 to clean them out");
			PrintLine("We will start a thread to do the cleaning, while we stop it later");

			LIMIT = 0;
			var t = Task.Factory.StartNew(
				new Action<object>(c => CleanSessions(c as IDatabase)),
				connection);
			Thread.Sleep(1.Seconds());
			QUIT = true;
			Thread.Sleep(2.Seconds());
			if (t.Status == TaskStatus.Running) {
				throw new Exception("The clean sessions thread is still alive?!?");
			}

			var s = connection.HashLength("login:");
			Assert.That(s, Is.EqualTo(0));
			PrintLine("The current number of sessions still available is: {0}", s);
		}

		[RIATest]
		public void TestShoppingCartCookies() {
			var token = Guid.NewGuid().ToString();
			PrintLine("We'll refresh our session...");
			UpdateToken(connection, token, "username", "itemX");

			PrintLine("And add an item to the shopping cart");
			AddToCart(connection, token, "itemY", 3);
			var r = connection.HashGetAll("cart:" + token);
			Assert.That(r, Is.Not.Null.Or.Empty);
			PrintLine("Our shopping cart currently has:");
			PrintHash(r);
			PrintBlank();

			PrintLine("Let's clean out our sessions and carts");
			LIMIT = 0;
			var t = Task.Factory.StartNew(
				new Action<object>(c => CleanFullSessions(c as IDatabase)),
				connection);
			Thread.Sleep(1.Seconds());
			QUIT = true;
			Thread.Sleep(2.Seconds());
			if (t.Status == TaskStatus.Running) {
				throw new Exception("The clean sessions thread is still alive?!?");
			}

			r = connection.HashGetAll("cart:" + token);
			Assert.That(r, Is.Empty);
			PrintLine("Our shopping cart now contains:");
			PrintHash(r);
		}

		[RIATest]
		public void TestCacheRequest() {
			var token = Guid.NewGuid().ToString();
			UpdateToken(connection, token, "username", "itemX");

			var url = "http://test.com/?item=itemX";
			PrintLine("Delete cached key, if exists.");
			connection.KeyDelete("cached:" + HashRequest(url));
			PrintLine("We are going to cache a simple request against: {0}", url);
			var result = CacheRequest(connection, url, req => "content for " + req);
			Assert.That(result, Is.Not.Null.Or.Empty);
			PrintLine("We got initial content: {0}", result);
			PrintBlank();

			PrintLine("To test that we've cached the request, we'll pass a bad callback");
			var result2 = CacheRequest(connection, url, req => null);
			PrintLine("We ended up getting the same response! {0}", result2);
			Assert.That(result, Is.EqualTo(result2));

			Assert.That(CanCache(connection, "http://test.com/"), Is.False);
			Assert.That(CanCache(connection, "http://test.com/?item=itemX&_=1234536"), Is.False);
		}

		[RIATest]
		public void TestCacheRows() {
			PrintLine("First, let's schedule caching of itemX every 5 seconds");
			ScheduleRowCache(connection, "itemX", 5);
			var s = connection.SortedSetRangeByRankWithScores("schedule:", 0, -1);
			Assert.That(s, Is.Not.Null.Or.Empty);
			PrintLine("Our schedule looks like:");
			PrintSortedSet(s);
			PrintBlank();

			PrintLine("We'll start a caching thread that will cache the data...");
			var t = Task.Factory.StartNew(
				new Action<object>(c => CacheRows(c as IDatabase)),
				connection);
			Thread.Sleep(1.Seconds());
			PrintLine("Our cached data looks like:");
			var r = connection.StringGet("inv:itemX");
			Assert.That(r.HasValue, Is.True);
			PrintLine(r);
			PrintBlank();

			PrintLine("We'll check again in 5 seconds...");
			Thread.Sleep(5.Seconds());

			PrintLine("Notice that the data has changed...");
			var r2 = connection.StringGet("inv:itemX");
			Assert.That(r2.HasValue, Is.True);
			Assert.That(r, Is.Not.EqualTo(r2));
			PrintLine(r2);
			PrintBlank();

			PrintLine("Let's force un-caching");
			ScheduleRowCache(connection, "itemX", -1);
			Thread.Sleep(1.Seconds());
			r = connection.StringGet("inv:itemX");
			Assert.That(r.HasValue, Is.False);
			PrintLine("The cache was cleared? {0}", r.HasValue ? "no" : "yes");

			QUIT = true;
			Thread.Sleep(2.Seconds());
			if (t.Status == TaskStatus.Running) {
				throw new Exception("The database caching thread is still alive?!?");
			}
		}
	}
}
