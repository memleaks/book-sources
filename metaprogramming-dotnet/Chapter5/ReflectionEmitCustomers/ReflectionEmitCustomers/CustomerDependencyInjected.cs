using Customers;
using ReflectionEmitCustomers.Builders;
using System;

namespace ReflectionEmitCustomers
{
	public sealed class CustomerDependencyInjected
		: Customer
	{
		public CustomerDependencyInjected(IToStringBuilder builder)
			: base()
		{
			this.Builder = builder;
		}

		public override string ToString()
		{
			return this.Builder.ToString(this);
		}

		private IToStringBuilder Builder { get; set; }
	}
}
