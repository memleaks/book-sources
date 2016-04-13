using Customers;
using ReflectionCustomers.Extensions;
using System;
using System.Reflection;

namespace ReflectionCustomers
{
	public sealed class CustomerReflection : Customer
	{
		public override string ToString()
		{
			return this.ToStringReflection();
		}
	}
}
