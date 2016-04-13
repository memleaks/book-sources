using System;

namespace DynamicMocks.Roslyn.Tests
{
	public sealed class TestCallback
	{
		public int Callback()
		{
			return new Random().Next();
		}
	}
}
