using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GeneticAlgorithm.Tests
{
	[TestClass]
	public sealed class GeneticAlgorithmTests
	{
		[TestMethod]
		public void Create()
		{
			var parameters = Substitute.For<IGeneticAlgorithmParameters<int>>();
			var ga = new GeneticAlgorithm<int>(parameters);
			Assert.AreSame(parameters, ga.Parameters);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullParameters()
		{
			new GeneticAlgorithm<int>(null);
		}

		[TestMethod]
		public void Run()
		{
			var population = new Population<int>(new List<Chromosome<int>> { new Chromosome<int>(0, 0.5), new Chromosome<int>(0, 0.5) });
			var solution = new Chromosome<int>(0, 0d);

			var parameters = Substitute.For<IGeneticAlgorithmParameters<int>>();
			parameters.GeneratePopulation().Returns(population);
			parameters.Terminator(Arg.Is<Population<int>>(population)).Returns(solution);

			var ga = new GeneticAlgorithm<int>(parameters);
			ga.Run();

			Assert.IsTrue(ga.WasOptimalSolutionFound);
			Assert.AreSame(ga.Final, population);
		}

		[TestMethod]
		public void RunOneGenerationWithSuccessfulTerminationAndCrossover()
		{
			var population = new Population<int>(
				new List<Chromosome<int>> { new Chromosome<int>(0, 0.5), new Chromosome<int>(0, 0.5) });

			var parameters = Substitute.For<IGeneticAlgorithmParameters<int>>();

			var terminatorCallCount = 0;
			parameters.Terminator(Arg.Any<Population<int>>()).Returns((_) =>
			{
				Chromosome<int> returnValue = null;

				if(terminatorCallCount > 0)
				{
					returnValue = new Chromosome<int>(0, 0d);
				}

				terminatorCallCount++;
				return returnValue;
			});
			parameters.Crossover(Arg.Any<ReadOnlyCollection<Chromosome<int>>>()).Returns(
				new List<int> { 0, 0 }.AsReadOnly());
			parameters.GeneratePopulation().Returns(population);
			parameters.FitnessEvaluator(Arg.Any<int>()).Returns(0.5);
			parameters.Mutator(Arg.Any<int>()).Returns(0);
			parameters.SelectFittestChildren(Arg.Any<Population<int>>()).Returns(
				new List<Chromosome<int>>().AsReadOnly());
			parameters.CrossoverProbability.Returns(2.0);
			parameters.NumberOfGenerationRuns.Returns(1);
			parameters.NumberOfGenerations.Returns(1);
			parameters.TaskCount.Returns(1);

			var ga = new GeneticAlgorithm<int>(parameters);
			var wasGenerationCompletedRaised = false;
			ga.GenerationCompleted += (sender, e) =>
			{
				wasGenerationCompletedRaised = true;
			};
			ga.Run();

			Assert.IsTrue(wasGenerationCompletedRaised);
			Assert.IsTrue(ga.WasOptimalSolutionFound);
		}

		[TestMethod]
		public void RunOneGenerationWithSuccessfulTerminationAndNoCrossover()
		{
			var population = new Population<int>(
				new List<Chromosome<int>> { new Chromosome<int>(0, 0.5), new Chromosome<int>(0, 0.5) });

			var parameters = Substitute.For<IGeneticAlgorithmParameters<int>>();

			var terminatorCallCount = 0;
			parameters.Terminator(Arg.Any<Population<int>>()).Returns((_) =>
			{
				Chromosome<int> returnValue = null;

				if(terminatorCallCount > 0)
				{
					returnValue = new Chromosome<int>(0, 0d);
				}

				terminatorCallCount++;
				return returnValue;
			});
			parameters.GeneratePopulation().Returns(population);
			parameters.FitnessEvaluator(Arg.Any<int>()).Returns(0.5);
			parameters.Mutator(Arg.Any<int>()).Returns(0);
			parameters.SelectFittestChildren(Arg.Any<Population<int>>()).Returns(
				new List<Chromosome<int>>().AsReadOnly());
			parameters.CrossoverProbability.Returns(0.0);
			parameters.NumberOfGenerationRuns.Returns(1);
			parameters.NumberOfGenerations.Returns(1);
			parameters.TaskCount.Returns(1);

			var ga = new GeneticAlgorithm<int>(parameters);
			var wasGenerationCompletedRaised = false;
			ga.GenerationCompleted += (sender, e) =>
			{
				wasGenerationCompletedRaised = true;
			};
			ga.Run();

			Assert.IsTrue(wasGenerationCompletedRaised);
			Assert.IsTrue(ga.WasOptimalSolutionFound);
		}

		[TestMethod]
		public void RunOneGenerationWithUnsuccessfulTermination()
		{
			var population = new Population<int>(
				new List<Chromosome<int>> { new Chromosome<int>(0, 0.5), new Chromosome<int>(0, 0.5) });

			var parameters = Substitute.For<IGeneticAlgorithmParameters<int>>();
			parameters.GeneratePopulation().Returns(population);
			parameters.FitnessEvaluator(Arg.Any<int>()).Returns(0.5);
			parameters.Mutator(Arg.Any<int>()).Returns(0);
			parameters.SelectFittestChildren(Arg.Any<Population<int>>()).Returns(
				new List<Chromosome<int>>().AsReadOnly());
			parameters.Terminator(Arg.Any<Population<int>>()).Returns(null, null);
			parameters.Terminator(Arg.Any<Population<int>>()).Returns(null, null);
			parameters.CrossoverProbability.Returns(0.0);
			parameters.NumberOfGenerationRuns.Returns(1);
			parameters.NumberOfGenerations.Returns(1);
			parameters.TaskCount.Returns(1);

			var ga = new GeneticAlgorithm<int>(parameters);
			var wasGenerationCompletedRaised = false;
			ga.GenerationCompleted += (sender, e) =>
			{
				wasGenerationCompletedRaised = true;
			};
			ga.Run();

			Assert.IsTrue(wasGenerationCompletedRaised);
			Assert.IsFalse(ga.WasOptimalSolutionFound);
		}

		[TestMethod]
		public void RunWhenSolutionIsNullAndNumberOfGenerationRunsIsZero()
		{
			var population = new Population<int>(
				new List<Chromosome<int>> { new Chromosome<int>(0, 0.5), new Chromosome<int>(0, 0.5) });

			var parameters = Substitute.For<IGeneticAlgorithmParameters<int>>();
			parameters.GeneratePopulation().Returns(population);
			parameters.Terminator(Arg.Is<Population<int>>(population)).Returns(null, null);
			parameters.NumberOfGenerationRuns.Returns(0);

			var ga = new GeneticAlgorithm<int>(parameters);
			ga.Run();

			Assert.IsFalse(ga.WasOptimalSolutionFound);
			Assert.AreSame(ga.Final, population);
		}

		[TestMethod]
		public void RunWhenNumberOfGenerationsIsZeroAndGenerationEventIsSetAndSolutionIsAlwaysNull()
		{
			var population = new Population<int>(
				new List<Chromosome<int>> { new Chromosome<int>(0, 0.5), new Chromosome<int>(0, 0.5) });

			var parameters = Substitute.For<IGeneticAlgorithmParameters<int>>();
			parameters.GeneratePopulation().Returns(population);
			parameters.Terminator(Arg.Is<Population<int>>(population)).Returns(null, null);
			parameters.NumberOfGenerationRuns.Returns(1);
			parameters.NumberOfGenerations.Returns(0);

			var ga = new GeneticAlgorithm<int>(parameters);
			var wasGenerationRunCompletedRaised = false;

			ga.GenerationRunCompleted += (sender, e) =>
			{
				wasGenerationRunCompletedRaised = true;
			};
			ga.Run();

			Assert.IsTrue(wasGenerationRunCompletedRaised);
			Assert.IsFalse(ga.WasOptimalSolutionFound);
			Assert.AreSame(ga.Final, population);
		}
	}
}
