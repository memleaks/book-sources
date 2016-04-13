using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Spackle;
using System;
using System.Collections.ObjectModel;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public class ExpressionEvolverGeneticAlgorithmParametersMutatorTests
	{
		[TestMethod]
		public void Mutate()
		{
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
					result = 0;
				}
				else
				{
					throw new InvalidOperationException("Too many Next(int) calls.");
				}

				nextCallCount++;
				return result;
			});
			random.NextDouble().Returns(0d);

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				new RandomObjectGenerator().Generate<ReadOnlyCollection<ExpressionEvolverResult>>(),
				random))
			{
				Assert.AreEqual("a => (a + a)", parameters.Mutator(a => 3 + a).ToString());
			}
		}

		[TestMethod]
		public void MutateWithNoMutation()
		{
			var random = Substitute.For<SecureRandom>();
			random.NextDouble().Returns(1d);

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				new RandomObjectGenerator().Generate<ReadOnlyCollection<ExpressionEvolverResult>>(),
				random))
			{
				Assert.AreEqual("a => (3 + a)", parameters.Mutator(a => 3 + a).ToString());
			}
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void MutateWithNullArgument()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a))
			{
				parameters.Mutator(null);
			}
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void MutateOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;
			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a)) { }

			parameters.Mutator(null);
		}
	}
}
