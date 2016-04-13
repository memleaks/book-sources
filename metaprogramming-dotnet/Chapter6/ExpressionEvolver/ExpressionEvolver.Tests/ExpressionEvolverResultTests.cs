using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spackle;
using System;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionEvolverResultTests
	{
		[TestMethod]
		public void CreateWithException()
		{
			var generator = new RandomObjectGenerator();
			var parameter = generator.Generate<double>();
			var exception = generator.Generate<ArithmeticException>();

			var evolverResult = new ExpressionEvolverResult(parameter, exception);

			Assert.AreEqual(parameter, evolverResult.Parameter);
			Assert.AreSame(exception, evolverResult.Exception);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullArgument()
		{
			new ExpressionEvolverResult(0d, null);
		}

		[TestMethod]
		public void CreateWithResult()
		{
			var generator = new RandomObjectGenerator();
			var parameter = generator.Generate<double>();
			var result = generator.Generate<double>();

			var evolverResult = new ExpressionEvolverResult(parameter, result);

			Assert.AreEqual(parameter, evolverResult.Parameter);
			Assert.AreEqual(result, evolverResult.Result);
			Assert.IsNull(evolverResult.Exception);
		}
	}
}
