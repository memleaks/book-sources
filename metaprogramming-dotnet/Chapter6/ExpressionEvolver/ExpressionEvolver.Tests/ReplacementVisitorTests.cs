using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace ExpressionEvolver.Tests
{
	[TestClass]
	public sealed class ReplacementVisitorTests
	{
		[TestMethod]
		public void Replace()
		{
			//Expression<Func<int, int>> evenExpression = x => x / 2;
			var evenParameter = Expression.Parameter(typeof(int), "x");
			var evenExpression = Expression.Lambda<Func<int, int>>(
				 Expression.Divide(evenParameter, Expression.Constant(2, typeof(int))),
				 evenParameter);

			//Expression<Func<int, int>> oddExpression = x => 3 * x + 1;
			var oddParameter = Expression.Parameter(typeof(int), "x");
			var oddExpression = Expression.Lambda<Func<int, int>>(
				 Expression.Add(Expression.Multiply(Expression.Constant(3, typeof(int)), oddParameter),
					  Expression.Constant(1, typeof(int))),
				 oddParameter);

			// WANT: x => ((3 * x) + 1) / 2
			var newEvenExpression = Expression.Lambda<Func<int, int>>(
				 new ReplacementVisitor().Transform(
					  evenExpression.Body, evenExpression.Parameters,
					  evenParameter, oddExpression.Body),
				 evenParameter);

			Assert.AreEqual("x => (((3 * x) + 1) / 2)", newEvenExpression.ToString());
		}

		[TestMethod]
		public void ReplaceAdd()
		{
			var add = Expression.Add(Expression.Constant(1, typeof(int)),
				Expression.Constant(2, typeof(int)));
			var sourceExpression = Expression.Lambda<Action>(add);
			var replacement = Expression.Constant(3, typeof(int));

			Assert.AreEqual(replacement, new ReplacementVisitor().Transform(sourceExpression.Body, null,
				add, replacement));
		}

		[TestMethod]
		public void ReplaceAddLeftPart()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var add = Expression.Add(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(add);
			var replacement = Expression.Constant(3, typeof(int));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				leftPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Add, result.NodeType);
			Assert.AreEqual(replacement, result.Left);
			Assert.AreEqual(rightPart, result.Right);
		}

		[TestMethod]
		public void ReplaceAddRightPart()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var add = Expression.Add(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(add);
			var replacement = Expression.Constant(3, typeof(int));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				rightPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Add, result.NodeType);
			Assert.AreEqual(leftPart, result.Left);
			Assert.AreEqual(replacement, result.Right);
		}

		[TestMethod]
		public void ReplaceConstant()
		{
			var constant = Expression.Constant(1, typeof(int));
			var sourceExpression = Expression.Lambda<Action>(constant);
			var replacement = Expression.Constant(2, typeof(int));

			Assert.AreEqual(replacement, new ReplacementVisitor().Transform(sourceExpression.Body, null,
				constant, replacement));
		}

		[TestMethod]
		public void ReplaceDivide()
		{
			var divide = Expression.Divide(Expression.Constant(1, typeof(int)),
				Expression.Constant(2, typeof(int)));
			var sourceExpression = Expression.Lambda<Action>(divide);
			var replacement = Expression.Constant(3, typeof(int));

			Assert.AreEqual(replacement, new ReplacementVisitor().Transform(sourceExpression.Body, null,
				divide, replacement));
		}

		[TestMethod]
		public void ReplaceDivideLeftPart()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var divide = Expression.Divide(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(divide);
			var replacement = Expression.Constant(3, typeof(int));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				leftPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Divide, result.NodeType);
			Assert.AreEqual(replacement, result.Left);
			Assert.AreEqual(rightPart, result.Right);
		}

		[TestMethod]
		public void ReplaceDivideRightPart()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var divide = Expression.Divide(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(divide);
			var replacement = Expression.Constant(3, typeof(int));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				rightPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Divide, result.NodeType);
			Assert.AreEqual(leftPart, result.Left);
			Assert.AreEqual(replacement, result.Right);
		}

		[TestMethod]
		public void ReplaceMultiply()
		{
			var multiply = Expression.Multiply(Expression.Constant(1, typeof(int)),
				Expression.Constant(2, typeof(int)));
			var sourceExpression = Expression.Lambda<Action>(multiply);
			var replacement = Expression.Constant(3, typeof(int));

			Assert.AreEqual(replacement, new ReplacementVisitor().Transform(sourceExpression.Body, null,
				multiply, replacement));
		}

		[TestMethod]
		public void ReplaceMultiplyLeftPart()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var multiply = Expression.Multiply(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(multiply);
			var replacement = Expression.Constant(3, typeof(int));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				leftPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Multiply, result.NodeType);
			Assert.AreEqual(replacement, result.Left);
			Assert.AreEqual(rightPart, result.Right);
		}

		[TestMethod]
		public void ReplaceMultiplyRightPart()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var multiply = Expression.Multiply(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(multiply);
			var replacement = Expression.Constant(3, typeof(int));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				rightPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Multiply, result.NodeType);
			Assert.AreEqual(leftPart, result.Left);
			Assert.AreEqual(replacement, result.Right);
		}

		[TestMethod]
		public void ReplaceParameterInAddOnLeft()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.Add(replacementParameter, Expression.Constant(1, typeof(int)));

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Left);
		}

		[TestMethod]
		public void ReplaceParameterInAddOnRight()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.Add(Expression.Constant(1, typeof(int)), replacementParameter);

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Right);
		}

		[TestMethod]
		public void ReplaceParameterInSubtractOnLeft()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.Subtract(replacementParameter, Expression.Constant(1, typeof(int)));

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Left);
		}

		[TestMethod]
		public void ReplaceParameterInSubtractOnRight()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.Subtract(Expression.Constant(1, typeof(int)), replacementParameter);

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Right);
		}

		[TestMethod]
		public void ReplaceParameterInMultiplyOnLeft()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.Multiply(replacementParameter, Expression.Constant(1, typeof(int)));

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Left);
		}

		[TestMethod]
		public void ReplaceParameterInMultiplyOnRight()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.Multiply(Expression.Constant(1, typeof(int)), replacementParameter);

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Right);
		}

		[TestMethod]
		public void ReplaceParameterInDivideOnLeft()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.Divide(replacementParameter, Expression.Constant(1, typeof(int)));

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Left);
		}

		[TestMethod]
		public void ReplaceParameterInDivideOnRight()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.Divide(Expression.Constant(1, typeof(int)), replacementParameter);

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Right);
		}

		[TestMethod]
		public void ReplaceParameterInPowerOnLeft()
		{
			var sourceParameter = Expression.Parameter(typeof(double), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<double>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(double), "a");
			var replacement = Expression.Power(replacementParameter, Expression.Constant(1d, typeof(double)));

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Left);
		}

		[TestMethod]
		public void ReplaceParameterInPowerOnRight()
		{
			var sourceParameter = Expression.Parameter(typeof(double), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<double>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(double), "a");
			var replacement = Expression.Power(Expression.Constant(1d, typeof(double)), replacementParameter);

			Assert.AreEqual(sourceParameter, (new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement) as BinaryExpression).Right);
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void ReplaceParameterInUnsupportedBinaryExpression()
		{
			var sourceParameter = Expression.Parameter(typeof(int), "a");
			var sourceBody = sourceParameter;
			var sourceExpression = Expression.Lambda<Action<int>>(sourceBody, sourceParameter);
			var replacementParameter = Expression.Parameter(typeof(int), "a");
			var replacement = Expression.ExclusiveOr(Expression.Constant(1, typeof(int)), replacementParameter);

			new ReplacementVisitor().Transform(sourceExpression.Body,
				sourceExpression.Parameters,
				sourceBody, replacement);
		}

		[TestMethod]
		public void ReplacePower()
		{
			var power = Expression.Power(Expression.Constant(1d, typeof(double)),
				Expression.Constant(2d, typeof(double)));
			var sourceExpression = Expression.Lambda<Action>(power);
			var replacement = Expression.Constant(3d, typeof(double));

			Assert.AreEqual(replacement, new ReplacementVisitor().Transform(sourceExpression.Body, null,
				power, replacement));
		}

		[TestMethod]
		public void ReplacePowerLeftPart()
		{
			var leftPart = Expression.Constant(1d, typeof(double));
			var rightPart = Expression.Constant(2d, typeof(double));
			var power = Expression.Power(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(power);
			var replacement = Expression.Constant(3d, typeof(double));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				leftPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Power, result.NodeType);
			Assert.AreEqual(replacement, result.Left);
			Assert.AreEqual(rightPart, result.Right);
		}

		[TestMethod]
		public void ReplacePowerRightPart()
		{
			var leftPart = Expression.Constant(1d, typeof(double));
			var rightPart = Expression.Constant(2d, typeof(double));
			var power = Expression.Power(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(power);
			var replacement = Expression.Constant(3d, typeof(double));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				rightPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Power, result.NodeType);
			Assert.AreEqual(leftPart, result.Left);
			Assert.AreEqual(replacement, result.Right);
		}

		[TestMethod]
		public void ReplaceSubtract()
		{
			var subtract = Expression.Subtract(Expression.Constant(1, typeof(int)),
				Expression.Constant(2, typeof(int)));
			var sourceExpression = Expression.Lambda<Action>(subtract);
			var replacement = Expression.Constant(3, typeof(int));

			Assert.AreEqual(replacement, new ReplacementVisitor().Transform(sourceExpression.Body, null,
				subtract, replacement));
		}

		[TestMethod]
		public void ReplaceSubtractLeftPart()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var subtract = Expression.Subtract(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(subtract);
			var replacement = Expression.Constant(3, typeof(int));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				leftPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Subtract, result.NodeType);
			Assert.AreEqual(replacement, result.Left);
			Assert.AreEqual(rightPart, result.Right);
		}

		[TestMethod]
		public void ReplaceSubtractRightPart()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var subtract = Expression.Subtract(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(subtract);
			var replacement = Expression.Constant(3, typeof(int));

			var result = new ReplacementVisitor().Transform(sourceExpression.Body, null,
				rightPart, replacement) as BinaryExpression;

			Assert.AreEqual(ExpressionType.Subtract, result.NodeType);
			Assert.AreEqual(leftPart, result.Left);
			Assert.AreEqual(replacement, result.Right);
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void ReplaceUnsupportedBinaryExpression()
		{
			var leftPart = Expression.Constant(1, typeof(int));
			var rightPart = Expression.Constant(2, typeof(int));
			var exclusiveOr = Expression.ExclusiveOr(leftPart, rightPart);
			var sourceExpression = Expression.Lambda<Action>(exclusiveOr);
			var replacement = Expression.Constant(3, typeof(int));

			new ReplacementVisitor().Transform(sourceExpression.Body, null,
				leftPart, replacement);
		}
	}
}
