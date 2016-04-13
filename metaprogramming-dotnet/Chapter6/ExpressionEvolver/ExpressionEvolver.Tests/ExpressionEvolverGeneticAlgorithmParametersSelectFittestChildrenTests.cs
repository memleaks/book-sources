using GeneticAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionEvolverGeneticAlgorithmParametersSelectFittestChildrenTests
	{
		[TestMethod]
		public void SelectFittestChildren()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; }))
			{
				var chromosomes = new List<Chromosome<Expression<Func<double, double>>>>();

				for(var i = 0; i < (int)ExpressionEvolverGeneticAlgorithmParameters.SelectFittestChildrenPercentage; i++)
				{
					chromosomes.Add(new Chromosome<Expression<Func<double, double>>>(a => a, (double)i));
				}

				var population = new Population<Expression<Func<double, double>>>(chromosomes);
				var results = parameters.SelectFittestChildren(population);

				Assert.AreEqual(1, results.Count);
				Assert.AreEqual(9d, results[0].Fitness);
			}
		}

		[TestMethod]
		public void SelectFittestChildrenWhenPopulationIsTooSmall()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; }))
			{
				var chromosomes = new List<Chromosome<Expression<Func<double, double>>>>();

				for(var i = 0; i < (int)ExpressionEvolverGeneticAlgorithmParameters.SelectFittestChildrenPercentage - 1; i++)
				{
					chromosomes.Add(new Chromosome<Expression<Func<double, double>>>(a => a, (double)i));
				}

				var population = new Population<Expression<Func<double, double>>>(chromosomes);
				var results = parameters.SelectFittestChildren(population);

				Assert.AreEqual(0, results.Count);
			}
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void SelectFittestChildrenOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;
			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; })) { }

			parameters.SelectFittestChildren(null);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void SelectFittestChildrenOnNullArgument()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; }))
			{
				parameters.SelectFittestChildren(null);
			}
		}
	}
}
