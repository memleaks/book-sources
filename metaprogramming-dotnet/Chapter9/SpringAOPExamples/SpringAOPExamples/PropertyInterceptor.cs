using System;
using System.Linq;
using System.Reflection;
using AopAlliance.Intercept;

namespace SpringAOPExamples
{
	public sealed class PropertyInterceptor
		: IMethodInterceptor
	{
		private static bool IsPropertyMethod(MethodBase method)
		{
			return (from property in method.DeclaringType.GetProperties(
						 BindingFlags.Public | BindingFlags.Instance)
					  where (property.GetGetMethod() == method ||
					  property.GetSetMethod() == method)
					  select property).Any();
		}

		public object Invoke(IMethodInvocation invocation)
		{
			if (PropertyInterceptor.IsPropertyMethod(invocation.Method))
			{
				Console.Out.WriteLine(
					"Property {0} was invoked.",
					invocation.Method.Name);
			}

			return invocation.Proceed();
		}
	}
}
