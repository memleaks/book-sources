using System;
using System.Diagnostics;

namespace TimingReflectionCalls
{
	public static class Program
	{
		static void Main(string[] args)
		{
			Program.TimeDirectCall();
			Program.TimeReflectionCall();
		}

		private static void TimeDirectCall()
		{
			var stopwatch = Stopwatch.StartNew();

			for(var x = 0; x < 500000; x++)
			{
				var random = new Random().Next();
			}

			stopwatch.Stop();
			Console.Out.WriteLine(stopwatch.Elapsed.ToString());
		}

		private static void TimeReflectionCall()
		{
			var stopwatch = Stopwatch.StartNew();

			for(var x = 0; x < 500000; x++)
			{
				var randomType = Type.GetType("System.Random");
				var nextMethod = randomType.GetMethod("Next", Type.EmptyTypes);
				var random = nextMethod.Invoke(
				  Activator.CreateInstance(randomType), null);
			}

			stopwatch.Stop();
			Console.Out.WriteLine(stopwatch.Elapsed.ToString());
		}
	}
}
