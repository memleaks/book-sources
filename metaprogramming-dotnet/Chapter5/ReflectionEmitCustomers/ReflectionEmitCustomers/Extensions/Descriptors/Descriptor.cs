using System;

namespace ReflectionEmitCustomers.Extensions.Descriptors
{
	internal abstract class Descriptor
	{
		protected Descriptor() 
			: base() { }

		protected internal string Value
		{
			get;
			protected set;
		}
	}
}
