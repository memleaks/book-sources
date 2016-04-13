using Customers;
using ReflectionEmitCustomers.Extensions;
using System;

namespace ReflectionEmitCustomers
{
	public sealed class CustomerReflectionEmit : Customer
	{
		public override string ToString()
		{
			return this.ToStringReflectionEmit();
		}
	}
}
