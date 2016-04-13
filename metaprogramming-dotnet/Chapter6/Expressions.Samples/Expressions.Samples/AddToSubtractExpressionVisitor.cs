using System.Linq.Expressions;

namespace Expressions.Samples
{
	internal sealed class AddToSubtractExpressionVisitor
		: ExpressionVisitor
	{
		internal Expression Change(Expression expression)
		{
			return this.Visit(expression);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			return node.NodeType == ExpressionType.Add ?
				Expression.Subtract(
					this.Visit(node.Left), this.Visit(node.Right)) :
				node;
		}
	}
}
