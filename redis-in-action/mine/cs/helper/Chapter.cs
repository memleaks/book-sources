using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StackExchange.Redis;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace helper
{
	[RiaTestFixture]
	public abstract class Chapter
	{
		#region Test Setup
		protected IDatabase connection = null;

		[TestFixtureSetUp]
		public void SetUp() {
			var file = Path.Combine(Directory.GetCurrentDirectory(), "helper.dll");
			var config = ConfigurationManager.OpenExeConfiguration(file);

			var hostSetting = config.AppSettings.Settings["redis-host"];
			string host = hostSetting == null ? "127.0.0.1" : hostSetting.Value;

			var dbSetting = config.AppSettings.Settings["redis-db"];
			int db;
			if (dbSetting == null || !int.TryParse(dbSetting.Value, out db)) {
				db = 0;
			}
			connection = ConnectionMultiplexer.Connect(host).GetDatabase(db);
		}
		#endregion

		#region Time Helpers
		protected long Now() {
			return Time(DateTime.Now);
		}

		protected long Time(DateTime time) {
			return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
		}
		#endregion

		#region Print Helpers
		public static void Print(string text) {
			Console.Write(text);
		}

		public static void Print(string format, params object[] args) {
			Console.Write(format, args);
		}

		public static void PrintLine(string line) {
			Console.WriteLine(line);
		}

		public static void PrintLine(string format, params object[] args) {
			Console.WriteLine(format, args);
		}

		public static void PrintBlank() {
			Console.WriteLine();
		}

		public static void PrintSpliter(string symbol = "-", string title = null, int width = 80) {
			int count = width;
			if (!string.IsNullOrEmpty(title)) {
				Print("[" + title + "] ");
				count = Math.Max(0, width - title.Length - 3); // 3 for '[', ']', ' '
			}
			PrintLine(symbol.Repeat(count));
		}

		public static void PrintHash(IEnumerable<HashEntry> hash) {
			PrintLine("{");
			bool empty = true;
			int max = 0;
			foreach (var entry in hash) {
				empty = false;
				max = Math.Max(max, entry.Name.ToString().Length);
			}
			if (empty) {
				PrintLine("  *** Empty ***");
			}
			else {
				string format = string.Format("  {{0,-{0}}} : {{1}}", max);
				foreach (var entry in hash) {
					PrintLine(format, entry.Name, entry.Value);
				}
			}
			PrintLine("}");
		}

		public static void PrintHashList(IEnumerable<RedisHash> hashList) {
			int number = 1;
			foreach (var hash in hashList) {
				PrintLine("- No.{0,2} -", number++);
				PrintHash(hash);
			}
		}

		public static void PrintSortedSet(IEnumerable<SortedSetEntry> sortedSet) {
			PrintLine("[");
			foreach (var entry in sortedSet) {
				PrintLine("  {0} : {1}", entry.Element, entry.Score);
			}
			PrintLine("]");
		}
		#endregion
	}

	public class RiaTestFixtureAttribute : TestFixtureAttribute, ITestAction
	{
		public ActionTargets Targets {
			get { return ActionTargets.Suite; }
		}

		public void BeforeTest(TestDetails testDetails) {
			Chapter.PrintBlank();
			Chapter.PrintSpliter(title: testDetails.Fixture.ToString());
			Chapter.PrintBlank();
		}

		public void AfterTest(TestDetails testDetails) {
			Chapter.PrintSpliter();
		}
	}

	public class RIATestAttribute : TestAttribute, ITestAction
	{
		public ActionTargets Targets {
			get { return ActionTargets.Test; }
		}

		public void BeforeTest(TestDetails testDetails) {
			Chapter.PrintSpliter(title: testDetails.Method.Name, width: 0);
		}

		public void AfterTest(TestDetails testDetails) {
			Chapter.PrintBlank();
		}
	}
}
