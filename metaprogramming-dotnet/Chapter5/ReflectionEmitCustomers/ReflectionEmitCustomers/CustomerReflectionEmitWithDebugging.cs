using Customers;
using ReflectionEmitCustomers.Extensions;
using System;

namespace ReflectionEmitCustomers
{
	public sealed class CustomerReflectionEmitWithDebugging : Customer
	{
		public override string ToString()
		{
			return this.ToStringReflectionEmitWithDebugging();
		}
	}
}
