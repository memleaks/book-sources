using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ReflectionEmitCustomers.Extensions.Descriptors
{
	internal sealed class MethodDescriptor : Descriptor
	{
		private MethodDescriptor()
			: base() { }

		internal MethodDescriptor(MethodBase method, Assembly containingAssembly)
			: base()
		{
			var parts = new List<string>();

			MethodDescriptor.AddLocation(method, parts);
			MethodDescriptor.AddCallingConventions(method, parts);
			MethodDescriptor.AddReturnValue(method, containingAssembly, parts);

			parts.Add(method.GetName(containingAssembly) +
				MethodDescriptor.GetMethodArgumentInformation(method, containingAssembly));

			this.Value = string.Join(" ", parts.ToArray());
		}

		private static void AddReturnValue(MethodBase method, Assembly containingAssembly, List<string> parts)
		{
			var info = method as MethodInfo;

			if(info != null && info.ReturnType != null)
			{
				parts.Add(new TypeDescriptor(info.ReturnType, containingAssembly).Value);
			}
			else
			{
				parts.Add("void");
			}
		}

		private static void AddLocation(MethodBase method, List<string> parts)
		{
			if(!method.IsStatic)
			{
				parts.Add("instance");
			}
		}

		private static void AddCallingConventions(MethodBase method, List<string> parts)
		{
			var callingConventions = method.GetCallingConventions();

			if(callingConventions.Length > 0)
			{
				parts.Add(callingConventions);
			}
		}

		private static string GetMethodArgumentInformation(MethodBase method, Assembly containingAssembly)
		{
			var information = new StringBuilder();

			information.Append("(");
			var i = 0;

			var argumentTypes = method.GetParameterTypes();

			if(argumentTypes.Length > 0)
			{
				var descriptors = new List<string>();

				foreach(var type in argumentTypes)
				{
					var argumentDescriptor = new List<string>() {
						new TypeDescriptor(type, containingAssembly).Value
					};

					descriptors.Add(string.Join(" ", argumentDescriptor.ToArray()));

					i++;
				}

				information.Append(string.Join(", ", descriptors.ToArray()));
			}

			information.Append(")");
			return information.ToString();
		}
	}
}
