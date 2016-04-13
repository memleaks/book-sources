using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionEvolverGeneticAlgorithmParametersCopyTests
	{
		[TestMethod]
		public void Copy()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; }))
			{
				Expression<Func<double, double>> expression = a => a;
				Assert.AreSame(expression, parameters.Copy(expression));
			}
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void CopyOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;
			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; })) { }

			parameters.Copy(a => a);
		}
	}
}
