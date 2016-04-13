using GeneticAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public class ExpressionEvolverGeneticAlgorithmParametersTerminatorTests
	{
		[TestMethod]
		public void TerminatorWithNoTermination()
		{
			var results = new List<ExpressionEvolverResult> { new ExpressionEvolverResult(1d, 1d) }.AsReadOnly();
			var population = new Population<Expression<Func<double, double>>>(
				new List<Chromosome<Expression<Func<double, double>>>> { 
					new Chromosome<Expression<Func<double, double>>>(a => a, -100d),
					new Chromosome<Expression<Func<double, double>>>(a => a, -100d)}.AsReadOnly());
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(results))
			{
				Assert.IsNull(parameters.Terminator(population));
			}
		}

		[TestMethod]
		public void TerminatorWithTermination()
		{
			var results = new List<ExpressionEvolverResult> { new ExpressionEvolverResult(1d, 1d) }.AsReadOnly();
			var population = new Population<Expression<Func<double, double>>>(
				new List<Chromosome<Expression<Func<double, double>>>> { 
					new Chromosome<Expression<Func<double, double>>>(a => a, 1d),
					new Chromosome<Expression<Func<double, double>>>(a => a, 1d) }.AsReadOnly());
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(results))
			{
				Assert.IsNotNull(parameters.Terminator(population));
			}
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void TerminatorOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;
			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; })) { }

			parameters.Terminator(null);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void TerminatorOnNullArgument()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; }))
			{
				parameters.Terminator(null);
			}
		}
	}
}
