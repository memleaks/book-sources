using ReflectionEmitCustomers.Extensions.Descriptors;
using System;
using System.Reflection;

namespace ReflectionEmitCustomers.Extensions
{
	internal static class MemberInfoExtensions
	{
		internal static string GetName(this MemberInfo @this, Assembly containingAssembly)
		{
			return new TypeDescriptor(
				@this.DeclaringType, containingAssembly, 
				@this.DeclaringType.IsGenericType).Value + "::" +
				@this.Name;
		}
	}
}
