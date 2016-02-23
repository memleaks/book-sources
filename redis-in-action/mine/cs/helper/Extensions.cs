using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;

namespace helper
{
	public static class Extensions
	{
		public static string Repeat(this char symbol, int count) {
			return new String(symbol, count);
		}

		public static string Repeat(this string symbol, int count) {
			return symbol.Length == 1
				? symbol[0].Repeat(count)
				: string.Concat(Enumerable.Repeat(symbol, count));
		}

		public static TimeSpan Seconds<T>(this T seconds) {
			return TimeSpan.FromSeconds(Convert.ToDouble(seconds));
		}

		public static bool IsNullOrEmpty(this Array array) {
			return array == null || array.Length == 0;
		}
	}
}
