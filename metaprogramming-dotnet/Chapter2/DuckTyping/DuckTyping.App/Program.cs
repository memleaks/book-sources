using System;
using System.Collections.Generic;

namespace DuckTyping.App
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Out.WriteLine(
				new Golfer().Call("Drive", "Reflection"));
			Console.Out.WriteLine(
				new RaceCarDriver().Call("Drive", "Reflection"));

			dynamic caller = new Golfer();
			Console.Out.WriteLine(
				caller.Drive("Dynamic"));

			var rangeOne = new Range(-10d, 10d);
			var rangeTwo = new Range(-5d, 15d);
			Console.Out.WriteLine(rangeOne + rangeTwo);
		}
	}

	public sealed class Range
	{
		public static Range operator +(Range a, Range b)
		{
			return new Range(Math.Min(a.Minimum, b.Minimum), 
				Math.Max(a.Maximum, b.Maximum));
		}

		public Range(double minimum, double maximum)
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		public override string ToString()
		{
			return string.Format("{0} : {1}", 
				this.Minimum, this.Maximum);
		}

		public double Maximum { get; private set; }
		public double Minimum { get; private set; }
	}

	public sealed class Golfer
	{
		public string Drive(string technique)
		{
			return technique + " - 300 yards";
		}
	}

	public sealed class RaceCarDriver
	{
		public string Drive(string technique)
		{
			return technique + " - 200 miles an hour";
		}
	}
}
