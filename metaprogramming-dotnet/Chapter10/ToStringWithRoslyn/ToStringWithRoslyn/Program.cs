using System;

namespace ToStringWithRoslyn
{
	class Program
	{
		static void Main(string[] args)
		{
			var person = new Person("Joe Smith", 30);
			Console.Out.WriteLine(person.ToString());
		}
	}
}
