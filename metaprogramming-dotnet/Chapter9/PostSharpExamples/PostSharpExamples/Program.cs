using System;

namespace PostSharpExamples
{
	public static class Program
	{
		private static void Main()
		{
			Program.TestCreationAttribute();

			Console.Out.WriteLine();

			Program.TestToStringAttribute();

			Console.Out.WriteLine();

			Program.TestEquals();

			Console.Out.WriteLine();

			Program.TestEquatable();
		}

		private static void TestToStringAttribute()
		{
			var noToString = new ClassWithoutToString();
			Console.Out.WriteLine("ClassWithoutToString: {0}", noToString.ToString());
			var toString = new ClassWithToString();
			Console.Out.WriteLine("ClassWithToString: {0}", toString.ToString());
		}

		private static void TestCreationAttribute()
		{
			var noData = new ClassWithCreation();
			Console.Out.WriteLine(noData.Data);
			var data = new ClassWithCreation(Guid.NewGuid());
			Console.Out.WriteLine(data.Data);

			var noDataNoCreation = new ClassWithoutCreation();
			Console.Out.WriteLine(noDataNoCreation.Data);
			var dataNoCreation = new ClassWithoutCreation(Guid.NewGuid());
			Console.Out.WriteLine(dataNoCreation.Data);
		}

		private static void TestEquals()
		{
			var equals1 = new ClassWithEquals { IntData = 10, StringData = "10" };
			var equals2 = new ClassWithEquals { IntData = 20, StringData = "20" };
			var equals3 = new ClassWithEquals { IntData = 10, StringData = "10" };

			Console.Out.WriteLine(equals1.Equals(equals2));
			Console.Out.WriteLine(equals1.Equals(equals3));
			Console.Out.WriteLine(equals1.GetHashCode());
			Console.Out.WriteLine(equals2.GetHashCode());
			Console.Out.WriteLine(equals3.GetHashCode());
		}

		private static void TestEquatable()
		{
			var equatable1 = new ClassWithEquatable { IntData = 10, StringData = "10" };
			var equatable2 = new ClassWithEquatable { IntData = 20, StringData = "20" };
			var equatable3 = new ClassWithEquatable { IntData = 10, StringData = "10" };

			Console.Out.WriteLine(equatable1.Equals(equatable2));
			Console.Out.WriteLine(equatable1.Equals(equatable3));
			Console.Out.WriteLine(equatable1.GetHashCode());
			Console.Out.WriteLine(equatable2.GetHashCode());
			Console.Out.WriteLine(equatable3.GetHashCode());
		}
	}
}
