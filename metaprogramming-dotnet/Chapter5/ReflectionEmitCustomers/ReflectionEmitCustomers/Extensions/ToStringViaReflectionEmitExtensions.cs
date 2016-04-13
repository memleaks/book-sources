using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ReflectionEmitCustomers.Extensions
{
	public static class ToStringViaReflectionEmitExtensions
	{
		private static Lazy<ReflectionEmitMethodGenerator> generator =
			new Lazy<ReflectionEmitMethodGenerator>();
		private static Dictionary<Type, Delegate> methods =
			new Dictionary<Type, Delegate>();

		internal static string ToStringReflectionEmit<T>(this T @this)
		{
			var targetType = @this.GetType();

			if(!ToStringViaReflectionEmitExtensions.methods.ContainsKey(targetType))
			{
				ToStringViaReflectionEmitExtensions.methods.Add(targetType,
					ToStringViaReflectionEmitExtensions.generator.Value.Generate<T>());
			}

			return (ToStringViaReflectionEmitExtensions.methods[targetType] as Func<T, string>)(@this);
		}
	}
}
