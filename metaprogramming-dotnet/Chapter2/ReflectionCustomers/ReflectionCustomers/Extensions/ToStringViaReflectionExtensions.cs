using Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReflectionCustomers.Extensions
{
	public static class ToStringViaReflectionExtensions
	{
		public static string ToStringReflection<T>(this T @this)
		{
			return string.Join(Constants.Separator,
				new List<string>(
					from prop in @this.GetType().GetProperties(
						BindingFlags.Instance | BindingFlags.Public)
					where prop.CanRead
					select string.Format("{0}: {1}", prop.Name, prop.GetValue(@this, null))).ToArray());
		}
	}
}
