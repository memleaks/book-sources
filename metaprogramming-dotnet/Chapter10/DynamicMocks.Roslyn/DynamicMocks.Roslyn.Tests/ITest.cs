using System;

namespace DynamicMocks.Roslyn.Tests
{
	public interface ITest
	{
		void CallMe(string data);
		int CallMe();
	}
}
