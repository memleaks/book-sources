using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ReflectionEmitCustomers.Extensions
{
	public static class ToStringViaReflectionEmitWithDebuggingExtensions
	{
		private static Lazy<ReflectionEmitWithDebuggingMethodGenerator> generator =
			new Lazy<ReflectionEmitWithDebuggingMethodGenerator>();
		private static Dictionary<Type, Delegate> methods =
			new Dictionary<Type, Delegate>();

		internal static string ToStringReflectionEmitWithDebugging<T>(this T @this)
		{
			var targetType = @this.GetType();

			if(!ToStringViaReflectionEmitWithDebuggingExtensions.methods.ContainsKey(targetType))
			{
				ToStringViaReflectionEmitWithDebuggingExtensions.methods.Add(targetType,
					ToStringViaReflectionEmitWithDebuggingExtensions.generator.Value.Generate<T>());
			}

			return (ToStringViaReflectionEmitWithDebuggingExtensions.methods[targetType] as Func<T, string>)(@this);
		}
	}
}
