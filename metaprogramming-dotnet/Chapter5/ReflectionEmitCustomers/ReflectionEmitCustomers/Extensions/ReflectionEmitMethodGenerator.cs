using System;
using System.Reflection.Emit;
using System.Reflection;

namespace ReflectionEmitCustomers.Extensions
{
	public sealed class ReflectionEmitMethodGenerator
	{
		private AssemblyBuilder Assembly { get; set; }
		private ModuleBuilder Module { get; set; }
		private AssemblyName Name { get; set; }

		public ReflectionEmitMethodGenerator()
			: base()
		{
			this.Name = new AssemblyName() { Name = Guid.NewGuid().ToString("N") };
			this.Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
				this.Name, AssemblyBuilderAccess.Run);
			this.Module = this.Assembly.DefineDynamicModule(this.Name.Name);
		}

		public Func<T, string> Generate<T>()
		{
			var target = typeof(T);
			var type = this.Module.DefineType(target.Namespace + "." + target.Name);
			var methodName = "ToString" + target.GetHashCode().ToString();
			var method = type.DefineMethod(methodName,
				MethodAttributes.Static | MethodAttributes.Public, 
				typeof(string), new Type[] { target });

			method.GetILGenerator().Generate(target);
			var createdType = type.CreateType();

			var createdMethod = createdType.GetMethod(methodName);
			return (Func<T, string>)Delegate.CreateDelegate(
				typeof(Func<T, string>), createdMethod);
		}
	}
}
