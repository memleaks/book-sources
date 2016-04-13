using ReflectionEmitCustomers.Extensions;
using System;

namespace ReflectionEmitCustomers.Builders
{
	public sealed class ToStringDynamicMethodBuilder
		: IToStringBuilder
	{
		public string ToString<T>(T target)
		{
			return target.ToStringDynamicMethod();
		}
	}
}
