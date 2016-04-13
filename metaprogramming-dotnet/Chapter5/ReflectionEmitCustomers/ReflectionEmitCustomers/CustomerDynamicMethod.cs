using Customers;
using ReflectionEmitCustomers.Extensions;
using System;

namespace ReflectionEmitCustomers
{
	public sealed class CustomerDynamicMethod : Customer
	{
		public override string ToString()
		{
			return this.ToStringDynamicMethod();
		}
	}
}
