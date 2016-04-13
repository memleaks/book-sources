using Spackle.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

namespace ExpressionEvolver
{
	public static class ExpressionExtensions
	{
		private const string ContentNaN = "NaN";
		private const string ContentInfinity = "Infinity";

		public static Expression Compress(this Expression @this)
		{
			@this.CheckParameterForNull("@this");

			var compressed = @this;

			if(typeof(BinaryExpression).IsAssignableFrom(compressed.GetType()))
			{
				var binary = compressed as BinaryExpression;

				compressed = ExpressionExtensions.HandleBothConstantsCase(compressed, binary);
				compressed = ExpressionExtensions.HandlePowerReductionCase(compressed, binary);
				compressed = ExpressionExtensions.HandleSubtractionOfSameValuesCase(compressed, binary);
				compressed = ExpressionExtensions.HandleDivideOfSameValuesCase(compressed, binary);
				compressed = ExpressionExtensions.HandleAddOfZero(compressed, binary);
				compressed = ExpressionExtensions.HandleSubtractOfZero(compressed, binary);
				compressed = ExpressionExtensions.HandleMultiplyOfZero(compressed, binary);
				compressed = ExpressionExtensions.HandleMultiplyOfOne(compressed, binary);

				binary = compressed as BinaryExpression;

				if(binary != null)
				{
					if(binary.Left.NodeType != ExpressionType.Constant &&
						binary.Left.NodeType != ExpressionType.Parameter)
					{
						compressed = (BinaryExpression)binary.Replace(null, binary.Left, binary.Left.Compress());
					}

					binary = compressed as BinaryExpression;

					if(binary.Right.NodeType != ExpressionType.Constant &&
						binary.Right.NodeType != ExpressionType.Parameter)
					{
						compressed = (BinaryExpression)binary.Replace(null, binary.Right, binary.Right.Compress());
					}

					binary = compressed as BinaryExpression;

					if(binary.Left.NodeType == ExpressionType.Constant &&
						binary.Right.NodeType == ExpressionType.Constant)
					{
						compressed = compressed.Compress();
					}
				}
			}

			if(@this.ToString() != compressed.ToString())
			{
				compressed = compressed.Compress();
			}

			return compressed;
		}

		public static Expression GetNode(this Expression @this, int location)
		{
			var currentCount = 0;
			return @this.GetNode(location, ref currentCount);
		}

		private static Expression GetNode(this Expression @this, int location, ref int currentCount)
		{
			@this.CheckParameterForNull("@this");

			Expression node = null;

			if(currentCount == location)
			{
				node = @this;
			}
			else if(@this.NodeType == ExpressionType.Lambda)
			{
				node = (@this as LambdaExpression).Body.GetNode(location, ref currentCount);
			}
			else if(typeof(BinaryExpression).IsAssignableFrom(@this.GetType()))
			{
				currentCount++;
				var binary = @this as BinaryExpression;

				node = binary.Left.GetNode(location, ref currentCount);

				if(node == null)
				{
					currentCount++;
					node = binary.Right.GetNode(location, ref currentCount);
				}
			}

			return node;
		}

		public static int GetNodeCount(this Expression @this)
		{
			@this.CheckParameterForNull("@this");

			var count = 0;

			if(@this.NodeType == ExpressionType.Lambda)
			{
				count += (@this as LambdaExpression).Body.GetNodeCount();
			}
			else if(typeof(BinaryExpression).IsAssignableFrom(@this.GetType()))
			{
				count++;
				var binary = @this as BinaryExpression;

				count += binary.Left.GetNodeCount();
				count += binary.Right.GetNodeCount();
			}
			else if(@this.NodeType == ExpressionType.Parameter ||
				@this.NodeType == ExpressionType.Constant)
			{
				count++;
			}

			return count;
		}

		public static bool IsValid(this Expression @this)
		{
			@this.CheckParameterForNull("@this");

			var nodeContent = @this.ToString();

			return !(nodeContent.Contains(ExpressionExtensions.ContentNaN) ||
				nodeContent.Contains(ExpressionExtensions.ContentInfinity));
		}

		public static Expression Replace(this Expression @this, ReadOnlyCollection<ParameterExpression> parameters,
			Expression find, Expression replacement)
		{
			return new ReplacementVisitor().Transform(@this, parameters, find, replacement);
		}

		private static Expression HandleAddOfZero(Expression reduced, BinaryExpression binary)
		{
			if(binary.NodeType == ExpressionType.Add &&
				((binary.Left.NodeType == ExpressionType.Constant &&
					(double)((binary.Left as ConstantExpression).Value) == 0d) ||
				(binary.Right.NodeType == ExpressionType.Constant &&
					(double)((binary.Right as ConstantExpression).Value) == 0d)))
			{
				reduced = binary.Left.NodeType == ExpressionType.Constant ?
					binary.Right : binary.Left;
			}
			return reduced;
		}

		private static Expression HandleSubtractOfZero(Expression reduced, BinaryExpression binary)
		{
			if(binary.NodeType == ExpressionType.Subtract &&
				((binary.Left.NodeType == ExpressionType.Constant &&
					(double)((binary.Left as ConstantExpression).Value) == 0d) ||
				(binary.Right.NodeType == ExpressionType.Constant &&
					(double)((binary.Right as ConstantExpression).Value) == 0d)))
			{
				reduced = binary.Left.NodeType == ExpressionType.Constant ?
					Expression.Multiply(Expression.Constant(-1d), binary.Right) : binary.Left;
			}
			return reduced;
		}

		private static Expression HandleMultiplyOfOne(Expression reduced, BinaryExpression binary)
		{
			Expression reducedExpression = null;

			if(binary.NodeType == ExpressionType.Multiply)
			{
				if(binary.Left.NodeType == ExpressionType.Constant &&
					(double)((binary.Left as ConstantExpression).Value) == 1d)
				{
					reducedExpression = binary.Right;
				}
				else if(binary.Right.NodeType == ExpressionType.Constant &&
					(double)((binary.Right as ConstantExpression).Value) == 1d)
				{
					reducedExpression = binary.Left;
				}
			}

			return reducedExpression == null ? reduced : reducedExpression;
		}

		private static Expression HandleMultiplyOfZero(Expression reduced, BinaryExpression binary)
		{
			if(binary.NodeType == ExpressionType.Multiply &&
				((binary.Left.NodeType == ExpressionType.Constant &&
					(double)((binary.Left as ConstantExpression).Value) == 0d) ||
				(binary.Right.NodeType == ExpressionType.Constant &&
					(double)((binary.Right as ConstantExpression).Value) == 0d)))
			{
				reduced = Expression.Constant(0d);
			}
			return reduced;
		}

		private static Expression HandleDivideOfSameValuesCase(Expression reduced, BinaryExpression binary)
		{
			if(binary.NodeType == ExpressionType.Divide &&
				binary.Left.ToString() == binary.Right.ToString())
			{
				reduced = Expression.Constant(1d);
			}
			return reduced;
		}

		private static Expression HandleSubtractionOfSameValuesCase(Expression reduced, BinaryExpression binary)
		{
			if(binary.NodeType == ExpressionType.Subtract &&
				binary.Left.ToString() == binary.Right.ToString())
			{
				reduced = Expression.Constant(0d);
			}
			return reduced;
		}

		private static Expression HandlePowerReductionCase(Expression reduced, BinaryExpression binary)
		{
			if(binary.NodeType == ExpressionType.Power &&
				binary.Right.NodeType == ExpressionType.Constant &&
				binary.Left.NodeType == ExpressionType.Power &&
				(binary.Left as BinaryExpression).Right.NodeType == ExpressionType.Constant)
			{
				reduced = Expression.Power((binary.Left as BinaryExpression).Left,
						Expression.Constant(
							(double)((binary.Right as ConstantExpression).Value) *
							(double)(((binary.Left as BinaryExpression).Right as ConstantExpression).Value)));
			}
			return reduced;
		}

		private static Expression HandleBothConstantsCase(Expression reduced, BinaryExpression binary)
		{
			if(binary.Left.NodeType == ExpressionType.Constant &&
				binary.Right.NodeType == ExpressionType.Constant)
			{
				double leftValue = (double)(binary.Left as ConstantExpression).Value;
				double rightValue = (double)(binary.Right as ConstantExpression).Value;
				ConstantExpression constant = null;

				switch(binary.NodeType)
				{
					case ExpressionType.Add:
						constant = Expression.Constant(leftValue + rightValue);
						break;
					case ExpressionType.Subtract:
						constant = Expression.Constant(leftValue - rightValue);
						break;
					case ExpressionType.Multiply:
						constant = Expression.Constant(leftValue * rightValue);
						break;
					case ExpressionType.Divide:
						constant = Expression.Constant(leftValue / rightValue);
						break;
					case ExpressionType.Power:
						constant = Expression.Constant(Math.Pow(leftValue, rightValue));
						break;
					default:
						throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
							"Unexpected expression type: {0}", binary.NodeType));
				}

				reduced = constant;
			}
			return reduced;
		}
	}
}
