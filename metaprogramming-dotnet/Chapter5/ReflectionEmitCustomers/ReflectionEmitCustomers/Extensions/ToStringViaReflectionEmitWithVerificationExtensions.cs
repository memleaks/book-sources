using AssemblyVerifier;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ReflectionEmitCustomers.Extensions
{
	public static class ToStringViaReflectionEmitWithVerificationExtensions
	{
		internal static string ToStringReflectionEmitWithVerification<T>(this T @this)
		{
			var target = @this.GetType();

			var name = new AssemblyName() { Name = "ToStringWithVerification" };
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
				name, AssemblyBuilderAccess.RunAndSave);
			var module = assembly.DefineDynamicModule(name.Name + ".dll");

			var fullTypeName = target.Namespace + "." + target.Name;
			var type = module.DefineType(target.Namespace + "." + target.Name, TypeAttributes.Public);
			var methodName = "ToString" + target.GetHashCode().ToString();
			var method = type.DefineMethod(methodName,
				MethodAttributes.Static | MethodAttributes.Public,
				typeof(string), new Type[] { target });

			var generator = method.GetILGenerator();
			//generator.Emit(OpCodes.Ldstr, fullTypeName);
			generator.Emit(OpCodes.Ret);

			var createdType = type.CreateType();

			assembly.Save(name.Name + ".dll");
			AssemblyVerification.Verify(assembly);

			var createdMethod = createdType.GetMethod(methodName);

			return ((Func<T, string>)Delegate.CreateDelegate(
				typeof(Func<T, string>), createdMethod))(@this);
		}
	}
}
