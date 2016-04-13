using System;
using System.Linq;
using System.Reflection;

namespace DuckTyping
{
	public static class ObjectExtensions
	{
		public static object Call(this object @this, 
			string methodName, 
			params object[] parameters)
		{
			var method = @this.GetType().GetMethod(methodName, 
				BindingFlags.Instance | BindingFlags.Public, null,
				Array.ConvertAll<object, Type>(
					parameters, target => target.GetType()), null); 
			return method.Invoke(@this, parameters);
		}
	}
}
