using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Expressions.Samples
{
	public static class ExpressionGenerators
	{
		public static int AddWithDebugging(int a, int b)
		{
			var name = Guid.NewGuid().ToString("N");
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName(name), AssemblyBuilderAccess.Run);
			var module = assembly.DefineDynamicModule(name, true);
			var type = module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Public);
			var methodName = Guid.NewGuid().ToString("N");
			var method = type.DefineMethod(methodName,
				MethodAttributes.Public | MethodAttributes.Static,
				typeof(int), new Type[] { typeof(int), typeof(int) });

			var generator = DebugInfoGenerator.CreatePdbGenerator();
			var document = Expression.SymbolDocument("AddDebug.txt");

			var x = Expression.Parameter(typeof(int));
			var y = Expression.Parameter(typeof(int));

			var addDebugInfo = Expression.DebugInfo(document,
				6, 9, 6, 22);
			var add = Expression.Add(x, y);
			var addBlock = Expression.Block(addDebugInfo, add);

			var lambda = Expression.Lambda(addBlock, x, y);
			lambda.CompileToMethod(method, generator);

			var bakedType = type.CreateType();
			return (int)bakedType.GetMethod(methodName)
			  .Invoke(null, new object[] { a, b });
		}

		public static int Add(int a, int b)
		{
			var x = Expression.Parameter(typeof(int));
			var y = Expression.Parameter(typeof(int));

			var add = Expression.Add(x, y);

			var lambda = Expression.Lambda(
				add, x, y);

			return (lambda.Compile() as Func<int, int, int>)
				(a, b);
		}

		public static int AddWithHandlers(int a, int b)
		{
			var x = Expression.Parameter(typeof(int));
			var y = Expression.Parameter(typeof(int));

			var lambda = Expression.Lambda(
				Expression.TryCatch(
					Expression.Block(
						Expression.AddChecked(x, y)),
					Expression.Catch(
						typeof(OverflowException),
						Expression.Constant(0))), x, y);

			return (lambda.Compile() as Func<int, int, int>)
				(a, b);
		}

		public static int Branching(bool doSwitch)
		{
			var @switch = Expression.Parameter(typeof(bool));
			var conditional = Expression.Condition(@switch,
				Expression.Constant(1),
				Expression.Constant(0));

			return (Expression.Lambda(conditional, @switch)
				.Compile() as Func<bool, int>)(doSwitch);
		}
	}
}
