using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Spackle;
using System;
using System.Linq.Expressions;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class RandomExpressionGeneratorTests
	{
		private static string GenerateExpression(int operationValue, bool isConstantPositive,
			bool isLeftConstant, bool isRightConstant)
		{
			const int maximumOperationCount = 1;
			const double injectConstantProbabilityValue = 0.5;
			var parameter = Expression.Parameter(typeof(double), "a");

			var random = Substitute.For<SecureRandom>();

			random.NextBoolean().Returns(isConstantPositive);
			var nextCallCount = 0;
			random.Next(Arg.Any<int>()).Returns((_) =>
			{
				var result = 0;

				if(nextCallCount == 0)
				{
					result = operationValue;
				}
				else
				{
					throw new InvalidOperationException("Too many Next(int) calls.");
				}

				nextCallCount++;
				return result;
			});

			var next2ArgCallCount = 0;
			random.Next(Arg.Any<int>(), Arg.Any<int>()).Returns((_) =>
				{
					var result = 0;

					if(next2ArgCallCount == 0)
					{
						result = 30;
					}
					else
					{
						throw new InvalidOperationException("Too many Next(int, int) calls.");
					}

					next2ArgCallCount++;
					return result;
				});

			var nextDoubleCallCount = 0;
			random.NextDouble().Returns((_) =>
			{
				var result = 0d;

				if(nextDoubleCallCount == 0)
				{
					result = isLeftConstant ? 0.3 : 0.7;
				}
				else if(nextDoubleCallCount == 1)
				{
					result = isRightConstant ? 0.3 : 0.7;
				}
				else if(nextDoubleCallCount == 2 && isLeftConstant && isRightConstant)
				{
					result = 0.3;
				}
				else
				{
					throw new InvalidOperationException("Too many NextDouble() calls.");
				}

				nextDoubleCallCount++;
				return result;
			});

			return new RandomExpressionGenerator(
				maximumOperationCount, injectConstantProbabilityValue,
				100d, parameter, random).Body.ToString();
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void CreateForUnsupportedOperation()
		{
			RandomExpressionGeneratorTests.GenerateExpression(5, false, true, false);
		}

		[TestMethod]
		public void CreateExpressionWhenBothSidesAreConstantsNegativeConstantAndParameter()
		{
			Assert.AreEqual("(-30 + a)",
				RandomExpressionGeneratorTests.GenerateExpression(0, false, true, true));
		}

		[TestMethod]
		public void CreateAddOfNegativeConstantAndParameter()
		{
			Assert.AreEqual("(-30 + a)",
				RandomExpressionGeneratorTests.GenerateExpression(0, false, true, false));
		}

		[TestMethod]
		public void CreateAddOfPositiveConstantAndParameter()
		{
			Assert.AreEqual("(30 + a)",
				RandomExpressionGeneratorTests.GenerateExpression(0, true, true, false));
		}

		[TestMethod]
		public void CreateAddOfParameterAndNegativeConstant()
		{
			Assert.AreEqual("(a + -30)",
				RandomExpressionGeneratorTests.GenerateExpression(0, false, false, true));
		}

		[TestMethod]
		public void CreateAddOfParameterAndPositiveConstant()
		{
			Assert.AreEqual("(a + 30)",
				RandomExpressionGeneratorTests.GenerateExpression(0, true, false, true));
		}

		[TestMethod]
		public void CreateSubtractOfNegativeConstantAndParameter()
		{
			Assert.AreEqual("(-30 - a)",
				RandomExpressionGeneratorTests.GenerateExpression(1, false, true, false));
		}

		[TestMethod]
		public void CreateSubtractOfPositiveConstantAndParameter()
		{
			Assert.AreEqual("(30 - a)",
				RandomExpressionGeneratorTests.GenerateExpression(1, true, true, false));
		}

		[TestMethod]
		public void CreateSubtractOfParameterAndNegativeConstant()
		{
			Assert.AreEqual("(a - -30)",
				RandomExpressionGeneratorTests.GenerateExpression(1, false, false, true));
		}

		[TestMethod]
		public void CreateSubtractOfParameterAndPositiveConstant()
		{
			Assert.AreEqual("(a - 30)",
				RandomExpressionGeneratorTests.GenerateExpression(1, true, false, true));
		}

		[TestMethod]
		public void CreateMultiplyOfNegativeConstantAndParameter()
		{
			Assert.AreEqual("(-30 * a)",
				RandomExpressionGeneratorTests.GenerateExpression(2, false, true, false));
		}

		[TestMethod]
		public void CreateMultiplyOfPositiveConstantAndParameter()
		{
			Assert.AreEqual("(30 * a)",
				RandomExpressionGeneratorTests.GenerateExpression(2, true, true, false));
		}

		[TestMethod]
		public void CreateMultiplyOfParameterAndNegativeConstant()
		{
			Assert.AreEqual("(a * -30)",
				RandomExpressionGeneratorTests.GenerateExpression(2, false, false, true));
		}

		[TestMethod]
		public void CreateMultiplyOfParameterAndPositiveConstant()
		{
			Assert.AreEqual("(a * 30)",
				RandomExpressionGeneratorTests.GenerateExpression(2, true, false, true));
		}

		[TestMethod]
		public void CreateDivideOfNegativeConstantAndParameter()
		{
			Assert.AreEqual("(-30 / a)",
				RandomExpressionGeneratorTests.GenerateExpression(3, false, true, false));
		}

		[TestMethod]
		public void CreateDivideOfPositiveConstantAndParameter()
		{
			Assert.AreEqual("(30 / a)",
				RandomExpressionGeneratorTests.GenerateExpression(3, true, true, false));
		}

		[TestMethod]
		public void CreateDivideOfParameterAndNegativeConstant()
		{
			Assert.AreEqual("(a / -30)",
				RandomExpressionGeneratorTests.GenerateExpression(3, false, false, true));
		}

		[TestMethod]
		public void CreateDivideOfParameterAndPositiveConstant()
		{
			Assert.AreEqual("(a / 30)",
				RandomExpressionGeneratorTests.GenerateExpression(3, true, false, true));
		}

		[TestMethod]
		public void CreateSquareRootOfParameter()
		{
			const int maximumOperationCount = 1;
			const double injectConstantProbabilityValue = 0d;
			var parameter = Expression.Parameter(typeof(double), "a");
			var random = Substitute.For<SecureRandom>();
			//random.Stub(e => e.Next(Arg<int>.Is.Anything)).Return(0).WhenCalled(i => i.ReturnValue = 4);
			random.Next(Arg.Any<int>()).Returns(4);

			var expressionGenerator = new RandomExpressionGenerator(
				maximumOperationCount, injectConstantProbabilityValue,
				100d, parameter, random);

			Assert.AreEqual("(a ^ 0.5)", expressionGenerator.Body.ToString());
		}

		[TestMethod]
		public void CreateParameterOnlyBody()
		{
			const int maximumOperationCount = 0;
			const double injectConstantProbabilityValue = 0d;
			var parameter = Expression.Parameter(typeof(double), "a");
			using(var random = new SecureRandom())
			{
				var expressionGenerator = new RandomExpressionGenerator(
					maximumOperationCount, injectConstantProbabilityValue,
					100d, parameter, random);

				Assert.AreEqual("a", expressionGenerator.Body.ToString());
			}
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullParameter()
		{
			using(var random = new SecureRandom())
			{
				new RandomExpressionGenerator(1, 0.5, 100d,
					null, random);
			}
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateWithNullRandom()
		{
			new RandomExpressionGenerator(1, 0.5, 100d,
				Expression.Parameter(typeof(double), "a"), null);
		}
	}
}
