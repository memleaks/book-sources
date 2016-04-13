using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ReflectionEmitCustomers.Extensions
{
	public static class ToStringViaDynamicMethodExtensions
	{
		private static Dictionary<Type, Delegate> methods =
			new Dictionary<Type, Delegate>();

		private static Func<T, string> CreateToStringViaDynamicMethod<T>()
		{
			var target = typeof(T);

			var toString = new DynamicMethod("ToString" + target.GetHashCode().ToString(),
				typeof(string), new Type[] { target });

			toString.GetILGenerator().Generate(target);
			return (Func<T, string>)toString.CreateDelegate(typeof(Func<T, string>));
		}

		internal static string ToStringDynamicMethod<T>(this T @this)
		{
			var targetType = @this.GetType();

			if(!ToStringViaDynamicMethodExtensions.methods.ContainsKey(targetType))
			{
				ToStringViaDynamicMethodExtensions.methods.Add(targetType,
					ToStringViaDynamicMethodExtensions.CreateToStringViaDynamicMethod<T>());
			}

			return (ToStringViaDynamicMethodExtensions.methods[targetType] as Func<T, string>)(@this);
		}
	}
}
