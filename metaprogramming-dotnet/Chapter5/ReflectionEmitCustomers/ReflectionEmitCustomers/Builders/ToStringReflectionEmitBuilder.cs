using ReflectionEmitCustomers.Extensions;
using System;

namespace ReflectionEmitCustomers.Builders
{
	public sealed class ToStringReflectionEmitBuilder
		: IToStringBuilder
	{
		public string ToString<T>(T target)
		{
			return target.ToStringReflectionEmit();
		}
	}
}
