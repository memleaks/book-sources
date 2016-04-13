using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Expressions.Samples
{
	internal static class MethodCreation
	{
		// f(x) = ((3 * x) / 2) + 4
		internal static Tuple<Func<double, double>, TimeSpan> CreateViaExpression()
		{
			var stopwatch = Stopwatch.StartNew();

			var parameter = Expression.Parameter(typeof(double));
			var method = Expression.Lambda(
				Expression.Add(
					Expression.Divide(
						Expression.Multiply(
							Expression.Constant(3d), parameter),
						Expression.Constant(2d)),
					Expression.Constant(4d)),
				parameter).Compile() as Func<double, double>;

			stopwatch.Stop();

			return new Tuple<Func<double, double>, TimeSpan>(
				method, stopwatch.Elapsed);
		}

		internal static Tuple<Func<double, double>, TimeSpan> CreateViaDynamicMethod()
		{
			var stopwatch = Stopwatch.StartNew();

			var method = new DynamicMethod("m",
				typeof(double), new Type[] { typeof(double) });
			var parameter = method.DefineParameter(
				1, ParameterAttributes.In, "x");
			var generator = method.GetILGenerator();
			generator.Emit(OpCodes.Ldc_R8, 3d);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Mul);
			generator.Emit(OpCodes.Ldc_R8, 2d);
			generator.Emit(OpCodes.Div);
			generator.Emit(OpCodes.Ldc_R8, 4d);
			generator.Emit(OpCodes.Add);
			generator.Emit(OpCodes.Ret);
			var compiledMethod = method.CreateDelegate(
				typeof(Func<double, double>)) as Func<double, double>;

			stopwatch.Stop();

			return new Tuple<Func<double, double>, TimeSpan>(
				compiledMethod, stopwatch.Elapsed);

		}
	}
}
