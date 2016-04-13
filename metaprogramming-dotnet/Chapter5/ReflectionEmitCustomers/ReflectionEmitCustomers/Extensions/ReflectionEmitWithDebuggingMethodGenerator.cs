using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics.SymbolStore;
using System.Diagnostics;
using System.IO;

namespace ReflectionEmitCustomers.Extensions
{
	public sealed class ReflectionEmitWithDebuggingMethodGenerator
	{
		private AssemblyBuilder Assembly { get; set; }
		private ModuleBuilder Module { get; set; }
		private AssemblyName Name { get; set; }

		public ReflectionEmitWithDebuggingMethodGenerator()
			: base()
		{
			this.Name = new AssemblyName() { Name = Guid.NewGuid().ToString("N") };
			this.Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
				this.Name, AssemblyBuilderAccess.Run);
			this.AddDebuggingAttribute(this.Assembly);
			this.Module = this.Assembly.DefineDynamicModule(this.Name.Name + ".dll", true);
		}

		private void AddDebuggingAttribute(AssemblyBuilder assembly)
		{
			var debugAttribute = typeof(DebuggableAttribute);
			var debugConstructor = debugAttribute.GetConstructor(
				 new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
			var debugBuilder = new CustomAttributeBuilder(
				 debugConstructor, new object[] { 
						DebuggableAttribute.DebuggingModes.DisableOptimizations | 
						DebuggableAttribute.DebuggingModes.Default });
			assembly.SetCustomAttribute(debugBuilder);
		}

		public Func<T, string> Generate<T>()
		{
			var target = typeof(T);
			var fileName = target.Name + "ToString.il";
			var document = this.Module.DefineDocument(fileName,
				SymDocumentType.Text, SymLanguageType.ILAssembly, 
				SymLanguageVendor.Microsoft);

			var type = this.Module.DefineType(target.Namespace + "." + target.Name);
			var methodName = "ToString" + target.GetHashCode().ToString();
			var method = type.DefineMethod(methodName,
				MethodAttributes.Static | MethodAttributes.Public,
				typeof(string), new Type[] { target });
			method.DefineParameter(1, ParameterAttributes.In, "target");

			using(var file = File.CreateText(fileName))
			{
				method.GetILGenerator().Generate(target, document, file);
			}

			var createdType = type.CreateType();

			var createdMethod = createdType.GetMethod(methodName);
			return (Func<T, string>)Delegate.CreateDelegate(
				typeof(Func<T, string>), createdMethod);
		}
	}
}
