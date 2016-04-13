using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spackle;
using System;

namespace GeneticAlgorithm.Tests
{
	[TestClass]
	public sealed class ChromosomeTests
	{
		[TestMethod]
		public void ChangeValue()
		{
			var generator = new RandomObjectGenerator();
			var value = generator.Generate<Guid>();
			var fitness = generator.Generate<double>();

			var chromosome = new Chromosome<Guid>(
				generator.Generate<Guid>(), fitness);
			chromosome.Value = value;

			Assert.AreEqual(value, chromosome.Value);
		}

		[TestMethod]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var value = generator.Generate<Guid>();
			var fitness = generator.Generate<double>();

			var chromosome = new Chromosome<Guid>(value, fitness);

			Assert.AreEqual(value, chromosome.Value);
			Assert.AreEqual(fitness, chromosome.Fitness);
		}
	}
}
