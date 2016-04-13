using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicMocks.Roslyn
{
	public static class Mock
	{
		private static readonly Lazy<ModuleBuilder> builder =
			new Lazy<ModuleBuilder>(() => Mock.CreateBuilder());

		public static T Create<T>(object callback)
			where T : class
		{
			var interfaceType = typeof(T);

			if (!interfaceType.IsInterface)
			{
				throw new NotSupportedException();
			}

			var callbackType = callback.GetType();
			var mockName = callbackType.Name +
				Guid.NewGuid().ToString("N");

			var template = new MockCodeGenerator(mockName,
				interfaceType, callbackType).Code;
			var compilation = Compilation.Create("Mock", 
				options: new CompilationOptions(OutputKind.DynamicallyLinkedLibrary),
				syntaxTrees: new[] 
				{ 
					SyntaxTree.ParseCompilationUnit(template) 
				},
				references: new MetadataReference[]
				{
					new AssemblyFileReference(typeof(Guid).Assembly.Location),
					new AssemblyFileReference(interfaceType.Assembly.Location),
					new AssemblyFileReference(callbackType.Assembly.Location)
				});
			var result = compilation.Emit(Mock.builder.Value);

			if (!result.Success)
			{
				throw new NotSupportedException(string.Join(Environment.NewLine,
					from diagnostic in result.Diagnostics
					select diagnostic.Info.GetMessage()));
			}

			return Activator.CreateInstance(Mock.builder.Value.GetType(mockName), callback) as T;
		}

		private static ModuleBuilder CreateBuilder()
		{
			var name = new AssemblyName { Name = Guid.NewGuid().ToString("N") };
			var builder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				name, AssemblyBuilderAccess.Run);
			return builder.DefineDynamicModule(name.Name);
		}
	}
}
