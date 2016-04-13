using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spackle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionEvolverGeneticAlgorithmParametersConstructionTests
	{
		[TestMethod]
		public void CreateWithFuncThrowingArithmeticException()
		{
			var exception = new ArithmeticException();
			var count = 0;
			Func<double, double> func = a =>
			{
				count++;
				throw exception;
			};

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(func, 1, 1d))
			{
				Assert.AreEqual(1, count);
				Assert.AreEqual(1, parameters.Results.Count);
				Assert.AreSame(exception, parameters.Results[0].Exception);
			}
		}

		[TestMethod]
		public void CreateWithFuncReturnNaNAndInfinity()
		{
			const double parameterResult = 3d;

			var count = 0;
			Func<double, double> func = a =>
			{
				var result = 0d;

				if(count == 0)
				{
					result = double.NaN;
				}
				else if(count == 1)
				{
					result = double.PositiveInfinity;
				}
				else
				{
					result = parameterResult;
				}

				count++;
				return result;
			};

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(func, 1, 1d))
			{
				Assert.AreEqual(3, count);
				Assert.AreEqual(1, parameters.Results.Count);
				Assert.AreEqual(parameterResult, parameters.Results[0].Result);
				Assert.AreEqual(-9.00000000000068E-06, parameters.AcceptableAverageMeanSquareError, 0.00000000001);
				Assert.AreEqual(9, parameters.ExceptionPenaltyFactor, 0.1);
			}
		}

		[TestMethod]
		public void CreateViaFunc()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => { return a; }))
			{
				Assert.AreEqual(ExpressionEvolverGeneticAlgorithmParameters.ResultGenerationCount,
					parameters.Results.Count);
			}
		}

		[TestMethod]
		public void CreateViaFuncAndRandom()
		{
			var random = new SecureRandom();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; }, random))
			{
				Assert.AreEqual(ExpressionEvolverGeneticAlgorithmParameters.ResultGenerationCount,
					parameters.Results.Count);
				Assert.AreSame(random, parameters.Random);
			}
		}

		[TestMethod]
		public void CreateViaFuncCountAndVariance()
		{
			const int count = 10;
			const double variance = 3d;

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				(a) => { return a; }, count, variance))
			{
				Assert.AreEqual(count, parameters.Results.Count);
				var range = new Range<double>(-1d * variance, variance);

				foreach(var result in parameters.Results)
				{
					Assert.AreEqual(result.Parameter, result.Result);

					if(result.Exception == null)
					{
						Assert.IsTrue(range.Contains(result.Result));
					}
				}
			}
		}

		[TestMethod]
		public void CreateViaFuncCountVarianceAndRandom()
		{
			const int count = 10;
			const double variance = 3d;
			var random = new SecureRandom();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => { return a; }, count, variance, random))
			{
				Assert.AreSame(random, parameters.Random);
				Assert.AreEqual(count, parameters.Results.Count);
				var range = new Range<double>(-1d * variance, variance);

				foreach(var result in parameters.Results)
				{
					Assert.AreEqual(result.Parameter, result.Result);

					if(result.Exception == null)
					{
						Assert.IsTrue(range.Contains(result.Result));
					}
				}
			}
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateViaNullFunc()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				null as Func<double, double>)) { }
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateViaFuncAndNullRandom()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a, null)) { }
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateViaNullFuncCountAndVariance()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				null, 0, 0d)) { }
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateViaFuncCountVarianceAndNullRandom()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a, 0, 0d, null)) { }
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void CreateViaFuncZeroCountAndVariance()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => { return a; }, 0, 0d)) { }
		}

		[TestMethod]
		public void CreateViaResults()
		{
			var generator = new RandomObjectGenerator();
			var results = generator.Generate<ReadOnlyCollection<ExpressionEvolverResult>>();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(results))
			{
				CollectionAssert.AreEquivalent(results, parameters.Results);
			}
		}

		[TestMethod]
		public void CreateViaResultsAndRandom()
		{
			var generator = new RandomObjectGenerator();
			var results = generator.Generate<ReadOnlyCollection<ExpressionEvolverResult>>();
			var random = new SecureRandom();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(results, random))
			{
				CollectionAssert.AreEquivalent(results, parameters.Results);
				Assert.AreSame(random, parameters.Random);
			}
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateViaNullResults()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				null as ReadOnlyCollection<ExpressionEvolverResult>)) { }
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateViaResultsAndNullRandom()
		{
			var generator = new RandomObjectGenerator();
			var results = generator.Generate<ReadOnlyCollection<ExpressionEvolverResult>>();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				results, null)) { }
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void CreateViaEmptyResults()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				new List<ExpressionEvolverResult>().AsReadOnly())) { }
		}
	}
}
