using System;

namespace ToStringWithRoslyn
{
	public sealed class Person
	{
		public Person(string name, uint age)
		{
			this.Name = name;
			this.Age = age;
		}

		public override string ToString()
		{
			return this.Generate();
		}

		public uint Age { get; private set; }
		public string Name { get; private set; }
	}
}
