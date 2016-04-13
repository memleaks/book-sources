using GeneticAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Spackle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionEvolverGeneticAlgorithmParametersCrossoverTests
	{
		[TestMethod]
		public void Crossover()
		{
			Expression<Func<double, double>> evenExpression = x => x / 2;
			Expression<Func<double, double>> oddExpression = x => 3 * x + 1;

			var random = Substitute.For<SecureRandom>();

			var nextCallCount = 0;
			random.Next(Arg.Any<int>()).Returns((_) =>
			{
				var result = 0;
				if(nextCallCount == 0)
				{
					result = 1;
				}
				else if(nextCallCount == 1)
				{
					result = 4;
				}
				else
				{
					throw new InvalidOperationException("Too many Next(int) calls.");
				}

				nextCallCount++;
				return result;
			});

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				new RandomObjectGenerator().Generate<ReadOnlyCollection<ExpressionEvolverResult>>(),
				random))
			{
				var children = parameters.Crossover(new List<Chromosome<Expression<Func<double, double>>>> {
					new Chromosome<Expression<Func<double, double>>>(evenExpression, 0d), 
					new Chromosome<Expression<Func<double, double>>>(oddExpression, 0d) }.AsReadOnly());

				Assert.AreEqual(2, children.Count);
				Assert.AreEqual("x => 0.5", children[0].ToString());
				Assert.AreEqual("x => ((3 * x) + x)", children[1].ToString());
			}
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CrossoverWithNullParents()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a))
			{
				parameters.Crossover(null);
			}
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void CrossoverWithEmptyParents()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a))
			{
				parameters.Crossover(new List<Chromosome<Expression<Func<double, double>>>>().AsReadOnly());
			}
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void CrossoverOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;
			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a)) { }

			parameters.Crossover(new List<Chromosome<Expression<Func<double, double>>>>().AsReadOnly());
		}
	}
}
