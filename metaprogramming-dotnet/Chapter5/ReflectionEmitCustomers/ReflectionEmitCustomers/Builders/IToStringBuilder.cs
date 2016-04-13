using System;

namespace ReflectionEmitCustomers.Builders
{
	public interface IToStringBuilder
	{
		string ToString<T>(T target);
	}
}
