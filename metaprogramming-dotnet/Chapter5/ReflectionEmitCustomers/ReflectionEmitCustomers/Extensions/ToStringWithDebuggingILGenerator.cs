using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Diagnostics.SymbolStore;
using System.IO;
using Customers;

namespace ReflectionEmitCustomers.Extensions
{
	internal static class ToStringWithDebuggingILGenerator
	{
		internal static void Generate(this ILGenerator @this,
			Type target, ISymbolDocumentWriter document, StreamWriter file)
		{
			var properties = target.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var lineNumber = 1;

			if(properties.Length > 0)
			{
				var stringBuilderType = typeof(StringBuilder);

				var toStringLocal = @this.DeclareLocal(typeof(StringBuilder));
				toStringLocal.SetLocalSymInfo("builder");

				@this.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes), 
					document, file, lineNumber++);
				@this.Emit(OpCodes.Stloc_0,
					document, file, lineNumber++);
				@this.Emit(OpCodes.Ldloc_0,
					document, file, lineNumber++);

				var appendMethod = stringBuilderType.GetMethod(
					"Append", new Type[] { typeof(string) });
				var toStringMethod = typeof(StringBuilder).GetMethod("ToString",
					Type.EmptyTypes);

				for(var i = 0; i < properties.Length; i++)
				{
					lineNumber = ToStringWithDebuggingILGenerator.CreatePropertyForToString(
						@this, properties[i], appendMethod,
						i < properties.Length - 1,
						document, file, lineNumber);
				}

				@this.Emit(OpCodes.Pop, document, file, lineNumber++);
				@this.Emit(OpCodes.Ldloc_0, document, file, lineNumber++);
				@this.Emit(OpCodes.Callvirt, toStringMethod, document, file, lineNumber++);
			}
			else
			{
				@this.Emit(OpCodes.Ldstr, string.Empty, document, file, lineNumber++);
			}

			@this.Emit(OpCodes.Ret, document, file, lineNumber++);
		}

		private static int CreatePropertyForToString(ILGenerator generator, PropertyInfo property,
			MethodInfo appendMethod, bool needsSeparator, 
			ISymbolDocumentWriter document, StreamWriter file, int lineNumber)
		{
			var propertyLineNumber = lineNumber;

			if(property.CanRead)
			{
				generator.Emit(OpCodes.Ldstr, property.Name + ": ", document, file, propertyLineNumber++);
				generator.Emit(OpCodes.Callvirt, appendMethod, document, file, propertyLineNumber++);
				generator.Emit(OpCodes.Ldarg_0, document, file, propertyLineNumber++);

				var propertyGet = property.GetGetMethod();

				generator.Emit(propertyGet.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
					propertyGet, document, file, propertyLineNumber++);

				var appendTyped = typeof(StringBuilder).GetMethod("Append",
					new Type[] { propertyGet.ReturnType });

				if(appendTyped.GetParameters()[0].ParameterType != propertyGet.ReturnType)
				{
					if(propertyGet.ReturnType.IsValueType)
					{
						generator.Emit(OpCodes.Box, propertyGet.ReturnType,
							document, file, propertyLineNumber++);
					}
				}

				generator.Emit(OpCodes.Callvirt, appendTyped,
					document, file, propertyLineNumber++);

				if(needsSeparator)
				{
					generator.Emit(OpCodes.Ldstr, Constants.Separator,
						document, file, propertyLineNumber++);
					generator.Emit(OpCodes.Callvirt, appendMethod,
						document, file, propertyLineNumber++);
				}
			}

			return propertyLineNumber;
		}
	}
}
