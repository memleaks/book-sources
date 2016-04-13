using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Diagnostics.SymbolStore;
using Customers;

namespace ReflectionEmitCustomers.Extensions
{
	internal static class ToStringILGenerator
	{
		internal static void Generate(this ILGenerator @this, Type target)
		{
			var properties = target.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if(properties.Length > 0)
			{
				var stringBuilderType = typeof(StringBuilder);

				var toStringLocal = @this.DeclareLocal(typeof(StringBuilder));

				@this.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
				@this.Emit(OpCodes.Stloc_0);
				@this.Emit(OpCodes.Ldloc_0);

				var appendMethod = stringBuilderType.GetMethod(
					"Append", new Type[] { typeof(string) });
				var toStringMethod = typeof(StringBuilder).GetMethod("ToString",
					Type.EmptyTypes);

				for(var i = 0; i < properties.Length; i++)
				{
					ToStringILGenerator.CreatePropertyForToString(
						@this, properties[i], appendMethod,
						i < properties.Length - 1);
				}

				@this.Emit(OpCodes.Pop);
				@this.Emit(OpCodes.Ldloc_0);
				@this.Emit(OpCodes.Callvirt, toStringMethod);
			}
			else
			{
				@this.Emit(OpCodes.Ldstr, string.Empty);
			}

			@this.Emit(OpCodes.Ret);
		}

		private static void CreatePropertyForToString(ILGenerator generator, PropertyInfo property,
			MethodInfo appendMethod, bool needsSeparator)
		{
			if(property.CanRead)
			{
				generator.Emit(OpCodes.Ldstr, property.Name + ": ");
				generator.Emit(OpCodes.Callvirt, appendMethod);
				generator.Emit(OpCodes.Ldarg_0);

				var propertyGet = property.GetGetMethod();

				generator.Emit(propertyGet.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
					propertyGet);

				var appendTyped = typeof(StringBuilder).GetMethod("Append",
					new Type[] { propertyGet.ReturnType });

				if(appendTyped.GetParameters()[0].ParameterType != propertyGet.ReturnType)
				{
					if(propertyGet.ReturnType.IsValueType)
					{
						generator.Emit(OpCodes.Box, propertyGet.ReturnType);
					}
				}

				generator.Emit(OpCodes.Callvirt, appendTyped);

				if(needsSeparator)
				{
					generator.Emit(OpCodes.Ldstr, Constants.Separator);
					generator.Emit(OpCodes.Callvirt, appendMethod);
				}
			}
		}
	}
}
