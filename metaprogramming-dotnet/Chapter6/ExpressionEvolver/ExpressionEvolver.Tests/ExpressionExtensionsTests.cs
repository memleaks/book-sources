using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ExpressionExtensionsTests
	{
		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CompressWithNullArgument()
		{
			(null as Expression).Compress();
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void CompressWithUnsupportedBinaryExpressionAndBothConstants()
		{
			// a => 3 ^ 5
			var body = Expression.GreaterThan(
				Expression.Constant(3d), Expression.Constant(5d));

			body.Compress();
		}

		[TestMethod]
		public void CompressWithDivideSameDividentAndDivisor()
		{
			// a => ((a + 3) / (a + 3))
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Divide(
				Expression.Add(parameter, Expression.Constant(3d)),
				Expression.Add(parameter, Expression.Constant(3d)));

			Assert.AreEqual("((a + 3) / (a + 3))", body.ToString());
			Assert.AreEqual("1", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithNestedPowers()
		{
			// a => (((a + 3) ^ 0.5) ^ 0.5)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Power(
				Expression.Power(
					Expression.Add(parameter, Expression.Constant(3d)),
					Expression.Constant(0.5d)),
				Expression.Constant(0.5d));

			Assert.AreEqual("(((a + 3) ^ 0.5) ^ 0.5)", body.ToString());
			Assert.AreEqual("((a + 3) ^ 0.25)", body.Compress().ToString());
		}

		[TestMethod]
		public void GetNodeOfLambda()
		{
			// a => (((a + 3) ^ 0.5) ^ 0.5)
			var target = Expression.Constant(3d);
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Lambda(
				Expression.Power(
					Expression.Power(
						Expression.Add(parameter, target),
						Expression.Constant(0.5d)),
					Expression.Constant(0.5d)), parameter);

			Assert.AreSame(target, body.GetNode(4));
		}

		[TestMethod]
		public void GetNodeWithConstantAsTarget()
		{
			// a => (((a + 3) ^ 0.5) ^ 0.5)
			var target = Expression.Constant(3d);
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Power(
				Expression.Power(
					Expression.Add(parameter, target),
					Expression.Constant(0.5d)),
				Expression.Constant(0.5d));

			Assert.AreSame(target, body.GetNode(4));
		}

		[TestMethod]
		public void GetNodeWithParameterAsTarget()
		{
			// a => (((a + 3) ^ 0.5) ^ 0.5)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Power(
				Expression.Power(
					Expression.Add(parameter, Expression.Constant(3d)),
					Expression.Constant(0.5d)),
				Expression.Constant(0.5d));

			Assert.AreSame(parameter, body.GetNode(3));
		}

		[TestMethod]
		public void GetNodeCountOfLambda()
		{
			// a => (((a + 3) ^ 0.5) ^ 0.5)
			var target = Expression.Constant(3d);
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Lambda(
				Expression.Power(
					Expression.Power(
						Expression.Add(parameter, target),
						Expression.Constant(0.5d)),
					Expression.Constant(0.5d)), parameter);

			Assert.AreEqual(7, body.GetNodeCount());
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetNodeCountWithNullArgument()
		{
			(null as Expression).GetNodeCount();
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetNodeWithNullArgument()
		{
			(null as Expression).GetNode(0);
		}

		[TestMethod]
		public void CompressSimpleExpressionWithUnnecessarySubtractionOfZeros()
		{
			// a => (((a * 3) - 0) - 0)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Subtract(
				Expression.Subtract(
					Expression.Multiply(
						parameter,
						Expression.Constant(3d)),
					Expression.Constant(0d)),
				Expression.Constant(0d));

			Assert.AreEqual("(((a * 3) - 0) - 0)", body.ToString());
			Assert.AreEqual("(a * 3)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressComplexExpressionWithUnnecessarySubtractionOfZeros()
		{
			// a => ((-13 / (-74 / ((((((a + 43) ^ 0.25) - 0) - 0) ^ a) + 81))) - -39)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Subtract(
				Expression.Divide(
					Expression.Constant(-13d),
					Expression.Divide(
						Expression.Constant(-74d),
						Expression.Add(
							Expression.Power(
								Expression.Subtract(
									Expression.Subtract(
										Expression.Power(
											Expression.Add(
												parameter, Expression.Constant(43d)),
											Expression.Constant(0.25)),
										Expression.Constant(0d)),
									Expression.Constant(0d)),
								parameter),
							Expression.Constant(81d)))),
				Expression.Constant(-39d));

			Assert.AreEqual("((-13 / (-74 / ((((((a + 43) ^ 0.25) - 0) - 0) ^ a) + 81))) - -39)", body.ToString());
			Assert.AreEqual("((-13 / (-74 / ((((a + 43) ^ 0.25) ^ a) + 81))) - -39)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressComplexExpressionForSubtractingSameValues()
		{
			// a => ((83 + ((((-32 - a) - (-32 - a)) - ((-32 - a) - (-32 - a))) + -76)) - a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Subtract(
				Expression.Add(
					Expression.Constant(83d),
					Expression.Add(
						Expression.Subtract(
							Expression.Subtract(
								Expression.Subtract(Expression.Constant(-32d), parameter),
								Expression.Subtract(Expression.Constant(-32d), parameter)),
							Expression.Subtract(
								Expression.Subtract(Expression.Constant(-32d), parameter),
								Expression.Subtract(Expression.Constant(-32d), parameter))),
						Expression.Constant(-76d))),
				parameter);

			Assert.AreEqual("((83 + ((((-32 - a) - (-32 - a)) - ((-32 - a) - (-32 - a))) + -76)) - a)", body.ToString());
			Assert.AreEqual("(7 - a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressComplexExpressionWithManyConstants()
		{
			// a => (((((-33 / -73) - -26) ^ 0.5) ^ 0.5) + ((11 - a) ^ 0.5))
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Add(
				Expression.Power(
					Expression.Power(
						Expression.Subtract(
							Expression.Divide(Expression.Constant(-33d), Expression.Constant(-73d)),
							Expression.Constant(-26d)),
						Expression.Constant(.5)),
					Expression.Constant(.5)),
					Expression.Power(
						Expression.Subtract(Expression.Constant(11d), parameter),
						Expression.Constant(.5)));

			Assert.AreEqual("(((((-33 / -73) - -26) ^ 0.5) ^ 0.5) + ((11 - a) ^ 0.5))", body.ToString());
			Assert.AreEqual("(2.26785275364294 + ((11 - a) ^ 0.5))", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithAddingOfZeroOnLeft()
		{
			// (a + 0)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Add(parameter, Expression.Constant(0d));

			Assert.AreEqual("(a + 0)", body.ToString());
			Assert.AreEqual("a", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithAddingOfZeroOnRight()
		{
			// (0 + a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Add(Expression.Constant(0d), parameter);

			Assert.AreEqual("(0 + a)", body.ToString());
			Assert.AreEqual("a", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithAddingOfConstantsOnLeft()
		{
			// ((3.4 + 7.2) * a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(
				Expression.Add(
					Expression.Constant(3.4d),
					Expression.Constant(7.2d)), parameter);

			Assert.AreEqual("((3.4 + 7.2) * a)", body.ToString());
			Assert.AreEqual("(10.6 * a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithAddingOfConstantsOnRight()
		{
			// (a * (3.4 + 7.2))
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(parameter,
				Expression.Add(
					Expression.Constant(3.4d),
					Expression.Constant(7.2d)));

			Assert.AreEqual("(a * (3.4 + 7.2))", body.ToString());
			Assert.AreEqual("(a * 10.6)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithAddingOfParameters()
		{
			// (a + a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Add(parameter, parameter);

			Assert.AreEqual("(a + a)", body.ToString());
			Assert.AreEqual("(a + a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithDividingOfConstantsOnLeft()
		{
			// ((3.4 / 2) * a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(
				Expression.Divide(
					Expression.Constant(3.4d),
					Expression.Constant(2.0d)), parameter);

			Assert.AreEqual("((3.4 / 2) * a)", body.ToString());
			Assert.AreEqual("(1.7 * a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithDividingOfConstantsOnRight()
		{
			// (a * (3.4 / 2))
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(parameter,
				Expression.Divide(
					Expression.Constant(3.4d),
					Expression.Constant(2.0d)));

			Assert.AreEqual("(a * (3.4 / 2))", body.ToString());
			Assert.AreEqual("(a * 1.7)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithDividingOfParameters()
		{
			// (a / b)
			var parameterA = Expression.Parameter(typeof(double), "a");
			var parameterB = Expression.Parameter(typeof(double), "b");
			var body = Expression.Divide(parameterA, parameterB);

			Assert.AreEqual("(a / b)", body.ToString());
			Assert.AreEqual("(a / b)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithMultiplyingOfOneOnLeft()
		{
			// (a * 1)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(parameter, Expression.Constant(1d));

			Assert.AreEqual("(a * 1)", body.ToString());
			Assert.AreEqual("a", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithMultiplyingOfOneOnRight()
		{
			// (1 * a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(Expression.Constant(1d), parameter);

			Assert.AreEqual("(1 * a)", body.ToString());
			Assert.AreEqual("a", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithMultiplyingOfZeroOnLeft()
		{
			// (a * 0)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(parameter, Expression.Constant(0d));

			Assert.AreEqual("(a * 0)", body.ToString());
			Assert.AreEqual("0", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithMultiplyingOfZeroOnRight()
		{
			// (0 * a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(Expression.Constant(0d), parameter);

			Assert.AreEqual("(0 * a)", body.ToString());
			Assert.AreEqual("0", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithMultiplyingOfConstantsOnLeft()
		{
			// ((3.4 * 2) * a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(
				Expression.Multiply(
					Expression.Constant(3.4d),
					Expression.Constant(2.0d)), parameter);

			Assert.AreEqual("((3.4 * 2) * a)", body.ToString());
			Assert.AreEqual("(6.8 * a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithMultiplyingOfConstantsOnRight()
		{
			// (a * (3.4 * 2))
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(parameter,
				Expression.Multiply(
					Expression.Constant(3.4d),
					Expression.Constant(2.0d)));

			Assert.AreEqual("(a * (3.4 * 2))", body.ToString());
			Assert.AreEqual("(a * 6.8)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithMultiplyingOfParameters()
		{
			// (a * a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(parameter, parameter);

			Assert.AreEqual("(a * a)", body.ToString());
			Assert.AreEqual("(a * a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithPowerOfConstantsOnLeft()
		{
			// ((3.4 ^ 2) * a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(
				Expression.Power(
					Expression.Constant(3.4d),
					Expression.Constant(2.0d)), parameter);

			Assert.AreEqual("((3.4 ^ 2) * a)", body.ToString());
			Assert.AreEqual("(11.56 * a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithPowerOfConstantsOnRight()
		{
			// (a * (3.4 ^ 2))
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(parameter,
				Expression.Power(
					Expression.Constant(3.4d),
					Expression.Constant(2.0d)));

			Assert.AreEqual("(a * (3.4 ^ 2))", body.ToString());
			Assert.AreEqual("(a * 11.56)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithPowerOfParameters()
		{
			// (a ^ a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Power(parameter, parameter);

			Assert.AreEqual("(a ^ a)", body.ToString());
			Assert.AreEqual("(a ^ a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithSubtractingOfZeroOnLeft()
		{
			// (a - 0)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Subtract(parameter, Expression.Constant(0d));

			Assert.AreEqual("(a - 0)", body.ToString());
			Assert.AreEqual("a", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithSubtractingOfZeroOnRight()
		{
			// (0 - a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Subtract(Expression.Constant(0d), parameter);

			Assert.AreEqual("(0 - a)", body.ToString());
			Assert.AreEqual("(-1 * a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithSubtractingWithSameExpressionOnBothSides()
		{
			// ((a + 3) - (a + 3))
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Subtract(
				Expression.Add(parameter, Expression.Constant(3d)),
				Expression.Add(parameter, Expression.Constant(3d)));

			Assert.AreEqual("((a + 3) - (a + 3))", body.ToString());
			Assert.AreEqual("0", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithSubtractingOfConstantsOnLeft()
		{
			// ((3.4 - 2) * a)
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(
				Expression.Subtract(
					Expression.Constant(3.4d),
					Expression.Constant(2.0d)), parameter);

			Assert.AreEqual("((3.4 - 2) * a)", body.ToString());
			Assert.AreEqual("(1.4 * a)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithSubtractingOfConstantsOnRight()
		{
			// (a * (3.4 - 2))
			var parameter = Expression.Parameter(typeof(double), "a");
			var body = Expression.Multiply(parameter,
				Expression.Subtract(
					Expression.Constant(3.4d),
					Expression.Constant(2.0d)));

			Assert.AreEqual("(a * (3.4 - 2))", body.ToString());
			Assert.AreEqual("(a * 1.4)", body.Compress().ToString());
		}

		[TestMethod]
		public void CompressWithSubtractingOfParameters()
		{
			// (a - b)
			var parameterA = Expression.Parameter(typeof(double), "a");
			var parameterB = Expression.Parameter(typeof(double), "b");
			var body = Expression.Subtract(parameterA, parameterB);

			Assert.AreEqual("(a - b)", body.ToString());
			Assert.AreEqual("(a - b)", body.Compress().ToString());
		}

		[TestMethod]
		public void IsValid()
		{
			// (a - b)
			var parameterA = Expression.Parameter(typeof(double), "a");
			var parameterB = Expression.Parameter(typeof(double), "b");
			var body = Expression.Subtract(parameterA, parameterB);

			Assert.IsTrue(body.IsValid());
		}

		[TestMethod]
		public void IsValidWithNaN()
		{
			Assert.IsFalse(Expression.Constant(double.NaN).IsValid());
		}

		[TestMethod]
		public void IsValidWithInfinity()
		{
			Assert.IsFalse(Expression.Constant(double.PositiveInfinity).IsValid());
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void IsValidWithNullArgument()
		{
			(null as Expression).IsValid();
		}
	}
}
