using System;

namespace Customers
{
	public interface ICustomer
	{
		int Age { get; set; }
		Guid Id { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
	}
}
