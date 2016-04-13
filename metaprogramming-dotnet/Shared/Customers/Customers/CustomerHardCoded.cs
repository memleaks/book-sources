using System;
using System.Text;

namespace Customers
{
	public sealed class CustomerHardCoded : Customer
	{
		public override string ToString()
		{
			return new StringBuilder()
				.Append("Age: ").Append(this.Age)
				.Append(Constants.Separator)
				.Append("Id: ").Append(this.Id)
				.Append(Constants.Separator)
				.Append("FirstName: ").Append(this.FirstName)
				.Append(Constants.Separator)
				.Append("LastName: ").Append(this.LastName).ToString();
		}
	}
}
