using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionEvolverGeneticAlgorithmParametersPropertyTests
	{
		[TestMethod]
		public void CheckProperties()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(a => a))
			{
				Assert.AreEqual(ExpressionEvolverGeneticAlgorithmParameters.CrossoverProbabilityValue,
					parameters.CrossoverProbability);
				Assert.AreEqual(ExpressionEvolverGeneticAlgorithmParameters.MutationProbabilityValue,
					parameters.MutationProbability);
				Assert.AreEqual(ExpressionEvolverGeneticAlgorithmParameters.RunCountValue,
					parameters.NumberOfGenerationRuns);
				Assert.AreEqual(ExpressionEvolverGeneticAlgorithmParameters.GenerationCountValue,
					parameters.NumberOfGenerations);
				Assert.AreEqual(ExpressionEvolverGeneticAlgorithmParameters.PopulationSizeValue,
					parameters.PopulationSize);
				Assert.AreEqual(Environment.ProcessorCount,
					parameters.TaskCount);

				try
				{
					var length = parameters.ChromosomeLength;
					Assert.Fail();
				}
				catch(NotImplementedException)
				{
				}
				catch
				{
					Assert.Fail();
				}
			}
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void CheckCrossoverProbabilityOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;

			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(a => a)) { }

			var value = parameters.CrossoverProbability;
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void CheckMutationProbabilityOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;

			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(a => a)) { }

			var value = parameters.MutationProbability;
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void CheckNumberOfGenerationRunsOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;

			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(a => a)) { }

			var value = parameters.NumberOfGenerationRuns;
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void CheckNumberOfGenerationsOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;

			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(a => a)) { }

			var value = parameters.NumberOfGenerations;
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void CheckPopulationSizeOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;

			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(a => a)) { }

			var value = parameters.PopulationSize;
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void CheckTaskCountOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;

			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(a => a)) { }

			var value = parameters.TaskCount;
		}
	}
}
