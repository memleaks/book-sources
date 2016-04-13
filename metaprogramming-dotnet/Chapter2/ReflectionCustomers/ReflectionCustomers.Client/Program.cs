using System;

namespace ReflectionCustomers.Client
{
	public static class Program
	{
		static void Main(string[] args)
		{
			Console.Out.WriteLine(new CustomerReflection()
			{
				Age = 20, FirstName = "Joe", Id = Guid.NewGuid(), LastName = "Smith"
			});
		}
	}
}
