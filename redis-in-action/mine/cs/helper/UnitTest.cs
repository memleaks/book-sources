using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.ConsoleRunner;
using System.Reflection;
using NUnit.Framework;

namespace helper
{
	public static class UnitTest
	{
		public static void Main() {
			Runner.Main(new[] {
				Assembly.GetEntryAssembly().Location,
				"/nologo",
				"/nodots",
				"/noresult",
				"/noshadow",
			});
		}
	}
}
