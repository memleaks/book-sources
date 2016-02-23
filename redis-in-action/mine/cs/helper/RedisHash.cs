using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;

namespace helper
{
	public class RedisHash : List<HashEntry>
	{
		public RedisHash(IEnumerable<HashEntry> data)
			: base(data) {
		}

		public RedisValue this[string key] {
			get {
				var entry = GetEntry(key);
				return entry == null ? RedisValue.Null : entry.Value.Value;
			}

			set {
				var entry = GetEntry(key);
				if (entry == null) {
					Add(new HashEntry(key, value));
				}
				else {
					this[IndexOf(entry.Value)] = new HashEntry(entry.Value.Name, value);
				}
			}
		}

		public HashEntry? GetEntry(string key) {
			foreach (var entry in this) {
				if (entry.Name == key) {
					return entry;
				}
			}
			return null;
		}
	}
}
