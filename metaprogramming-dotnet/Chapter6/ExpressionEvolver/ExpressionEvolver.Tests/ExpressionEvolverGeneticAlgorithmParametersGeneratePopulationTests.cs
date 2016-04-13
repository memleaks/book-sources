using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Spackle;
using System;
using System.Collections.Generic;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionEvolverGeneticAlgorithmParametersGeneratePopulationTests
	{
		[TestMethod]
		public void GeneratePopulation()
		{
			var random = Substitute.For<SecureRandom>();
			random.Next(Arg.Any<int>()).Returns(0);

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				new List<ExpressionEvolverResult> { new ExpressionEvolverResult(1d, 1d) }.AsReadOnly(),
				random))
			{
				var population = parameters.GeneratePopulation();
				Assert.AreEqual(population.Chromosomes.Count,
					ExpressionEvolverGeneticAlgorithmParameters.PopulationSizeValue);

				foreach(var chromosome in population.Chromosomes)
				{
					Assert.AreEqual("a => a", chromosome.Value.ToString());
					Assert.AreEqual(0d, chromosome.Fitness);
				}
			}
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void GeneratePopulationOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;
			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; })) { }

			parameters.GeneratePopulation();
		}
	}
}
