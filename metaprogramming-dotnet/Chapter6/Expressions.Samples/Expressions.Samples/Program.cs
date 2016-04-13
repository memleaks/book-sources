using Customers;
using ReflectionEmitCustomers;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Expressions.Samples
{
	internal static class Program
	{
		private const int Interations = 2000000;

		static void Main(string[] args)
		{
			//Program.RunExpressions();
			//Console.Out.WriteLine();
			//Program.TestMethodCreation();
			//Console.Out.WriteLine();
			//Program.TimeMethodCreation();
			//Console.Out.WriteLine();
			//Program.TimeTest();
			Program.ChangeAddToSubtract();
		}

		private static void ChangeAddToSubtract()
		{
			Expression<Func<int, int, int>> simpleAdd = (x, y) => x + y;
			Console.Out.WriteLine(simpleAdd.ToString());
			var simpleSubtract = new AddToSubtractExpressionVisitor().Change(simpleAdd);
			Console.Out.WriteLine(simpleSubtract.ToString());

			Expression<Func<int, int, int>> complexAdd = (x, y) => ((32 * x) / 4) + y + (x + 4);
			Console.Out.WriteLine(complexAdd.ToString());
			var complexSubtract = new AddToSubtractExpressionVisitor().Change(complexAdd);
			Console.Out.WriteLine(complexSubtract.ToString());
		}

		private static void RunExpressions()
		{
			Console.Out.WriteLine(
				"Add: " +
				ExpressionGenerators.Add(2, 3));

			Console.Out.WriteLine(
				"AddWithDebugging: " +
				ExpressionGenerators.AddWithDebugging(2, 3));

			Console.Out.WriteLine(
				"AddWithHandlers, 2 + 3: " +
				ExpressionGenerators.AddWithHandlers(2, 3));
			Console.Out.WriteLine(
				"AddWithHandlers, int.MaxValue + int.MaxValue: " +
				ExpressionGenerators.AddWithHandlers(int.MaxValue, int.MaxValue));

			Console.Out.WriteLine(
				"Branching: true is " +
				ExpressionGenerators.Branching(true));
			Console.Out.WriteLine(
				"Branching: false is " +
				ExpressionGenerators.Branching(false));
		}

		private static void TimeMethodCreation()
		{
			const int Iterations = 10000;

			Program.TimeMethodCreationViaDynamicMethod(Iterations);

			Console.Out.WriteLine();

			Program.TimeMethodCreationViaExpression(Iterations);
		}

		private static void TimeMethodCreationViaDynamicMethod(int Iterations)
		{
			var dynamicMethodTime = TimeSpan.Zero;

			for(var i = 0; i < Iterations; i++)
			{
				dynamicMethodTime += MethodCreation.CreateViaDynamicMethod().Item2;
			}

			Console.Out.WriteLine("DynamicMethod time: " + dynamicMethodTime.ToString());
		}

		private static void TimeMethodCreationViaExpression(int Iterations)
		{
			var expressionTime = TimeSpan.Zero;

			for(var i = 0; i < Iterations; i++)
			{
				expressionTime += MethodCreation.CreateViaExpression().Item2;
			}

			Console.Out.WriteLine("Expression time: " + expressionTime.ToString());
		}

		private static void TestMethodCreation()
		{
			var expressionResult = MethodCreation.CreateViaExpression();
			Console.Out.WriteLine("Expression result, f(4): " + expressionResult.Item1(4));
			Console.Out.WriteLine("Expression result, f(5): " + expressionResult.Item1(5));
			Console.Out.WriteLine("Expression result, f(6): " + expressionResult.Item1(6));

			Console.Out.WriteLine();

			var dynamicMethodResult = MethodCreation.CreateViaDynamicMethod();
			Console.Out.WriteLine("DynamicMethod result, f(4): " + dynamicMethodResult.Item1(4));
			Console.Out.WriteLine("DynamicMethod result, f(5): " + dynamicMethodResult.Item1(5));
			Console.Out.WriteLine("DynamicMethod result, f(6): " + dynamicMethodResult.Item1(6));
		}

		private static void TimeTest()
		{
			Program.TimeToString<CustomerDynamicMethod>();
			Program.TimeToString<CustomerExpressions>();
		}

		private static void TimeToString<T>() where T : ICustomer, new()
		{
			for(int i = 0; i < 100; i++)
			{
				var customer = new T()
				{
					FirstName = "Jason",
					LastName = "Timing"
				};

				var toString = customer.ToString();
			}

			var watch = Stopwatch.StartNew();

			for(int i = 0; i < Program.Interations; i++)
			{
				var customer = new T()
				{
					FirstName = "Jason",
					LastName = "Timing"
				};

				var toString = customer.ToString();
			}

			watch.Stop();

			Console.Out.WriteLine("Type: " + typeof(T).Name +
				", Time: " + watch.Elapsed.ToString());
		}
	}
}
