using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace LazyIntegers
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var lazyInteger = new Lazy<int>(() =>
			{
				return new Random().Next();
			});

			//Console.Out.WriteLine(lazyInteger.Value);

var randomType = typeof(Random);
var nextMethod = randomType.GetMethod("Next",
	new Type[] { typeof(int), typeof(int) });
var random = Activator.CreateInstance(randomType);
Console.Out.WriteLine(nextMethod.Invoke(random, 
	new object[] { 0, 10 }));
		}
	}

	public sealed class EventData : EventArgs { }

	public sealed class EventRaiser
	{
		public EventHandler<EventData> RaiseDataEvent;

		public void RaiseIt()
		{
			var @event = this.RaiseDataEvent;

			if(@event != null)
			{
				@event(this, new EventData());
			}
		}
	}
}
