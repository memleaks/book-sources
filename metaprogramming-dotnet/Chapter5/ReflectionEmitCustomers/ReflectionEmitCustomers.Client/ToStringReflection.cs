using System;
using System.Reflection;

namespace DynamicToString
{
	public static class ToStringReflection
	{
		public static string RunToString(object target)
		{
			return string.Join(" || ", 
				Array.ConvertAll<PropertyInfo, string>(
				target.GetType().GetProperties(
				BindingFlags.Instance | BindingFlags.Public),
				prop => prop.CanRead ? 
					string.Format("{0} : {1}", prop.Name, prop.GetValue(target, null)) : string.Empty));		
		}
	}
}
