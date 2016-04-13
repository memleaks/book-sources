using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DynamicToString
{
	internal delegate string ToStringDelegate<T>(T target);
	
	public static class ToStringDynamicMethod
	{
		private const string Separator = " || ";
		
		private static Dictionary<Type, Delegate> toStrings =
			new Dictionary<Type, Delegate>();
			
		public static string RunToString<T>(T target)
		{
			ToStringDelegate<T> toString = null;

			Type targetType = target.GetType();

			if(ToStringDynamicMethod.toStrings.ContainsKey(targetType))
			{
				toString = ToStringDynamicMethod.toStrings[targetType] as ToStringDelegate<T>;
			}
			else
			{
				toString = ToStringDynamicMethod.CreateToString(target);
				ToStringDynamicMethod.toStrings.Add(targetType, toString);
			}

			return toString(target);
		}
		
		/// <summary>
		/// Links to keep in mind:
		/// Visualizer: http://blogs.msdn.com/haibo_luo/archive/2005/10/25/484861.aspx
		/// Debugging: http://blogs.msdn.com/yirutang/archive/2005/05/26/422373.aspx
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		/// <returns></returns>
		private static ToStringDelegate<T> CreateToString<T>(T target)
		{
			Type type = typeof(T);
			
			DynamicMethod toString = new DynamicMethod("ToString" + typeof(T).GetHashCode().ToString(), 
				typeof(string), new Type[] { typeof(T) }, typeof(ToStringDynamicMethod).Module);
			
			ILGenerator generator = toString.GetILGenerator();
			
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if(properties.Length > 0)
			{
				Type stringBuilderType = typeof(StringBuilder);
				
				LocalBuilder toStringLocal = generator.DeclareLocal(typeof(StringBuilder));
				
				generator.Emit(OpCodes.Newobj, stringBuilderType.GetConstructor(Type.EmptyTypes));
				generator.Emit(OpCodes.Stloc_0);
				generator.Emit(OpCodes.Ldloc_0);
				
				MethodInfo appendMethod = stringBuilderType.GetMethod(
					"Append", new Type[] { typeof(string) });
				MethodInfo toStringMethod = typeof(object).GetMethod("ToString");
				
				for(int i = 0; i < properties.Length; i++)
				{
					PropertyInfo property = properties[i];
					
					if(property.CanRead)
					{
						generator.Emit(OpCodes.Ldstr, property.Name + ": ");
						generator.EmitCall(OpCodes.Callvirt, appendMethod, null);
						generator.Emit(OpCodes.Ldarg_0);

						MethodInfo propertyGet = property.GetGetMethod();

						generator.EmitCall(propertyGet.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
							propertyGet, null);

						Type returnType = propertyGet.ReturnType;
						if(returnType != typeof(string))
						{
							if(returnType.IsValueType)
							{
								LocalBuilder localReturnType = generator.DeclareLocal(returnType);
								generator.Emit(OpCodes.Stloc, localReturnType);
								generator.Emit(OpCodes.Ldloca, localReturnType);
							}
							
							MethodInfo returnToStringMethod = returnType.GetMethod("ToString", Type.EmptyTypes);
							generator.EmitCall(OpCodes.Callvirt, returnToStringMethod ?? toStringMethod, null);
						}

						generator.EmitCall(OpCodes.Callvirt, appendMethod, null);

						if(i < properties.Length - 1)
						{
							generator.Emit(OpCodes.Ldstr, ToStringDynamicMethod.Separator);
							generator.EmitCall(OpCodes.Callvirt, appendMethod, null);
						}					
					}
				}

				generator.Emit(OpCodes.Pop);
				generator.Emit(OpCodes.Ldloc_0);
				generator.EmitCall(OpCodes.Callvirt, toStringMethod, null);
			}
			else
			{
				generator.Emit(OpCodes.Ldstr, string.Empty);
			}			
			
			generator.Emit(OpCodes.Ret);
			
			return (ToStringDelegate<T>)toString.CreateDelegate(typeof(ToStringDelegate<T>));
		}
	}
}