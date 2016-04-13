using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace GeneticAlgorithm.Tests
{
	[TestClass]
	public sealed class PopulationTests
	{
		[TestMethod]
		public void Create()
		{
			var chromosomes = new List<Chromosome<Guid>>();
			var chromosome = new Chromosome<Guid>(Guid.NewGuid(), 0.5);
			chromosomes.Add(chromosome);
			chromosomes.Add(new Chromosome<Guid>(Guid.NewGuid(), 0.5));

			var population = new Population<Guid>(chromosomes);

			Assert.AreEqual(1.0, population.FitnessSummary);
			Assert.AreEqual(0.5, population.FitnessAverage);
			Assert.AreEqual(2, population.Chromosomes.Count);

			var firstChromosome = population.Chromosomes[0];
			Assert.AreSame(chromosome, firstChromosome);
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void CreateWhenPopulationIsEmpty()
		{
			new Population<Guid>(new List<Chromosome<Guid>>());
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateWhenPopulationIsNull()
		{
			new Population<Guid>(null);
		}
	}
}
