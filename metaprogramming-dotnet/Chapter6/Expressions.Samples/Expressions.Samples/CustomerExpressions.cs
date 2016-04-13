using Customers;
using Expressions.Samples.Extensions;

namespace Expressions.Samples
{
	public sealed class CustomerExpressions : Customer
	{
		public override string ToString()
		{
			return this.ToStringExpression();
		}
	}
}
