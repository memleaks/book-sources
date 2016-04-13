using Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Expressions.Samples.Extensions
{
	internal static class ToStringViaExpressionsExtensions
	{
		private static Dictionary<Type, Delegate> methods =
			new Dictionary<Type, Delegate>();

		internal static string ToStringExpression<T>(this T @this)
		{
			var targetType = @this.GetType();

			if(!ToStringViaExpressionsExtensions.methods.ContainsKey(targetType))
			{
				ToStringViaExpressionsExtensions.methods.Add(targetType,
					ToStringViaExpressionsExtensions.CreateToStringViaExpression(@this));
			}

			return (ToStringViaExpressionsExtensions.methods[targetType] as Func<T, string>)(@this);
		}

		private static Func<T, string> CreateToStringViaExpression<T>(T target)
		{
			var builder = typeof(StringBuilder);
			var builderCtor = builder.GetConstructor(Type.EmptyTypes);
			var append = builder.GetMethod("Append", new Type[] { typeof(string) });
			var toString = builder.GetMethod("ToString", Type.EmptyTypes);

			var thisParameter = Expression.Parameter(typeof(T), "@this");
			Expression body = Expression.New(builder.GetConstructor(Type.EmptyTypes));

			var properties = new List<PropertyInfo>(
				from prop in target.GetType().GetProperties(
					BindingFlags.Instance | BindingFlags.Public)
				where prop.CanRead
				select prop);

			for(var i = 0; i < properties.Count; i++)
			{
				var property = properties[i];
				body = Expression.Call(body, append,
					Expression.Constant(property.Name + ": "));
				var typedAppend = builder.GetMethod("Append", new Type[] { property.PropertyType });

				if(typedAppend.GetParameters()[0].ParameterType == property.PropertyType)
				{
					body = Expression.Call(body, typedAppend,
						Expression.Call(thisParameter, property.GetGetMethod()));
				}
				else
				{
					body = Expression.Call(body, typedAppend,
						Expression.TypeAs(
							Expression.Call(thisParameter, property.GetGetMethod()),
								typedAppend.GetParameters()[0].ParameterType));
				}

				if(i < properties.Count - 1)
				{
					body = Expression.Call(body, append,
						Expression.Constant(Constants.Separator));
				}
			}

			body = Expression.Call(body, toString);

			return Expression.Lambda<Func<T, string>>(body, thisParameter).Compile();
		}
	}
}
