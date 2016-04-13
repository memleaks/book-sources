using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DynamicMocks.Roslyn
{
	public sealed class MockCodeGenerator
	{
		private const string Template = @"
			[System.Serializable]
			internal sealed class {0}
				: {1}
			{{ 
				private {2} callback;
				
				public {0}({2} callback)
				{{
					this.callback = callback;
				}}

				{3} 
			}}";

		public MockCodeGenerator(string mockName,
			Type interfaceType, Type callbackType)
			: base()
		{
			this.MockName = mockName;
			this.InterfaceType = interfaceType;
			this.InterfaceTypeName = InterfaceType.FullName;
			this.CallbackType = callbackType;
			this.Generate();
		}

		private void Generate()
		{
			this.Code = string.Format(MockCodeGenerator.Template,
				this.MockName, this.InterfaceTypeName,
				this.CallbackType.FullName,
				this.GetMethods());
		}

		private MethodInfo FindMethod(
			MethodInfo[] callbackMethods, MethodInfo interfaceMethod)
		{
			MethodInfo result = null;

			foreach (var callbackMethod in callbackMethods)
			{
				if (callbackMethod.ReturnType ==
					interfaceMethod.ReturnType)
				{
					var callbackParameters =
						callbackMethod.GetParameters();
					var interfaceParameters =
						interfaceMethod.GetParameters();

					if (callbackParameters.Length ==
						interfaceParameters.Length)
					{
						var foundDifference = false;

						for (var i = 0; i < interfaceParameters.Length; i++)
						{
							if (callbackParameters[0].ParameterType !=
								interfaceParameters[0].ParameterType)
							{
								foundDifference = true;
								break;
							}
						}

						if (!foundDifference)
						{
							result = callbackMethod;
							break;
						}
					}
				}
			}

			return result;
		}

		private static string GetMethod(MethodInfo method, bool includeTypes = true)
		{
			var result = new StringBuilder();

			if (includeTypes)
			{
				result.Append(method.ReturnType == typeof(void) ? "void " :
					method.ReturnType.FullName + " ");
			}

			result.Append(method.Name + "(");
			result.Append(string.Join(", ",
				from parameter in method.GetParameters()
				select (includeTypes ?
					parameter.ParameterType.FullName + " " + parameter.Name :
					parameter.Name)));
			result.Append(")");
			return result.ToString();
		}

		private string GetMethods()
		{
			var methods = new StringBuilder();
			var callbackMethods = this.CallbackType.GetMethods(
				BindingFlags.Public | BindingFlags.Instance);

			foreach (var interfaceMethod in this.InterfaceType.GetMethods())
			{
				methods.Append("public " +
					MockCodeGenerator.GetMethod(interfaceMethod) + "{");

				var callbackMethod = this.FindMethod(callbackMethods, interfaceMethod);

				if (callbackMethod != null)
				{
					if (callbackMethod.ReturnType != typeof(void))
					{
						methods.Append("return ");
					}

					methods.Append("this.callback." +
						MockCodeGenerator.GetMethod(callbackMethod, false) + ";");
				}
				else
				{
					if (interfaceMethod.ReturnType != typeof(void))
					{
						methods.Append("return " + (
							interfaceMethod.ReturnType.IsClass ?
							"null;" : string.Format("default({0});",
								interfaceMethod.ReturnType.FullName)));
					}
				}

				methods.Append("}");
			}

			return methods.ToString();
		}

		private Type CallbackType { get; set; }
		public string Code { get; private set; }
		private Type InterfaceType { get; set; }
		private string MockName { get; set; }
		private string InterfaceTypeName { get; set; }
	}
}
