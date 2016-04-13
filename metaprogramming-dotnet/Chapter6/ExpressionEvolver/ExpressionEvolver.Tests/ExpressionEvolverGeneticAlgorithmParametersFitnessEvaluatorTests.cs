using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionEvolverGeneticAlgorithmParametersFitnessEvaluatorTests
	{
		[TestMethod]
		public void EvaluateFitness()
		{
			var results = new List<ExpressionEvolverResult> {
				new ExpressionEvolverResult(1d, 1d) }.AsReadOnly();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				results))
			{
				Assert.AreEqual(0d, parameters.FitnessEvaluator(a => a));
			}
		}

		[TestMethod]
		public void EvaluateFitnessWhenResultIsNaN()
		{
			var results = new List<ExpressionEvolverResult> {
				new ExpressionEvolverResult(1d, 1d) }.AsReadOnly();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				results))
			{
				Assert.AreEqual(-1d,
					parameters.FitnessEvaluator(a => double.NaN));
			}
		}

		[TestMethod]
		public void EvaluateFitnessWhenResultExpectationIsException()
		{
			var results = new List<ExpressionEvolverResult> {
				new ExpressionEvolverResult(1d, new ArithmeticException()) }.AsReadOnly();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				results))
			{
				Assert.AreEqual(0d, parameters.FitnessEvaluator(a => a));
			}
		}

		[TestMethod]
		public void EvaluateFitnessWithExceptionAndResultExpectationIsException()
		{
			var results = new List<ExpressionEvolverResult> {
				new ExpressionEvolverResult(0d, new ArithmeticException()) }.AsReadOnly();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				results))
			{
				Assert.AreEqual(0d, parameters.FitnessEvaluator(a => (double)(1 / (int)a)));
			}
		}

		[TestMethod]
		public void EvaluateFitnessWithExceptionAndResultExpectationIsNotException()
		{
			var results = new List<ExpressionEvolverResult> {
				new ExpressionEvolverResult(0d, 0d) }.AsReadOnly();

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				results))
			{
				Assert.AreEqual(0,
					parameters.FitnessEvaluator(a => (double)(1 / (int)a)));
			}
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void EvaluateFitnessWithNullArgument()
		{
			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a))
			{
				parameters.FitnessEvaluator(null);
			}
		}

		[TestMethod, ExpectedException(typeof(ObjectDisposedException))]
		public void EvaluateFitnessOnDisposedObject()
		{
			ExpressionEvolverGeneticAlgorithmParameters parameters = null;
			using(parameters = new ExpressionEvolverGeneticAlgorithmParameters(
				a => a)) { }

			parameters.FitnessEvaluator(null);
		}
	}
}
