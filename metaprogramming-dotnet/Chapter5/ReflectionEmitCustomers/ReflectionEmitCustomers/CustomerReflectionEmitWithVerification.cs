using Customers;
using ReflectionEmitCustomers.Extensions;
using System;

namespace ReflectionEmitCustomers
{
	public sealed class CustomerReflectionEmitWithVerification 
		: Customer
	{
		public override string ToString()
		{
			return this.ToStringReflectionEmitWithVerification();
		}
	}
}
