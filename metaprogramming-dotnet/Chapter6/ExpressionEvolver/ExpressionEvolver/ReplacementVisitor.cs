using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionEvolver
{
	public sealed class ReplacementVisitor
		 : ExpressionVisitor
	{
		public Expression Transform(Expression source,
			 ReadOnlyCollection<ParameterExpression> sourceParameters,
			 Expression find, Expression replacement)
		{
			this.Find = find;

			if(sourceParameters != null)
			{
				this.Replacement = new ParameterReplacementVisitor()
					.Transform(sourceParameters, replacement);
			}
			else
			{
				this.Replacement = replacement;
			}

			return this.Visit(source);
		}

		private Expression ReplaceNode(Expression node)
		{
			if(node == this.Find)
			{
				return this.Replacement;
			}
			else
			{
				return node;
			}
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			var result = this.ReplaceNode(node);

			if(result == node)
			{
				switch(node.NodeType)
				{
					case ExpressionType.Add:
						result = Expression.Add(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					case ExpressionType.Subtract:
						result = Expression.Subtract(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					case ExpressionType.Multiply:
						result = Expression.Multiply(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					case ExpressionType.Divide:
						result = Expression.Divide(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					case ExpressionType.Power:
						result = Expression.Power(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					default:
						throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
							"Unexpected expression type: {0}", node.NodeType));
				}
			}

			return result;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			return this.ReplaceNode(node);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			return this.ReplaceNode(node);
		}

		private Expression Find { get; set; }
		private Expression Replacement { get; set; }

		private sealed class ParameterReplacementVisitor
			: ExpressionVisitor
		{
			public Expression Transform(ReadOnlyCollection<ParameterExpression> sourceParameters,
				 Expression replacement)
			{
				this.SourceParameters = sourceParameters;
				return this.Visit(replacement);
			}

			protected override Expression VisitBinary(BinaryExpression node)
			{
				Expression result = null;

				switch(node.NodeType)
				{
					case ExpressionType.Add:
						result = Expression.Add(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					case ExpressionType.Subtract:
						result = Expression.Subtract(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					case ExpressionType.Multiply:
						result = Expression.Multiply(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					case ExpressionType.Divide:
						result = Expression.Divide(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					case ExpressionType.Power:
						result = Expression.Power(
							this.Visit(node.Left), this.Visit(node.Right));
						break;
					default:
						throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
							"Unexpected expression type: {0}", node.NodeType));
				}

				return result;
			}

			protected override Expression VisitParameter(ParameterExpression node)
			{
				return (from sourceParameter in this.SourceParameters
						  where sourceParameter.Name == node.Name
						  select sourceParameter).First();
			}

			private ReadOnlyCollection<ParameterExpression> SourceParameters { get; set; }
		}
	}
}
