namespace DynamicMocks.Roslyn.Tests
{
	public interface ISimpleInterface
	{
		void NoReturnValueAndArguments(string param1, int param2);
		void NoReturnValueAndNoArguments();
		string ReferenceReturnValueAndArguments(string param1, int param2);
		string ReferenceReturnValueAndNoArguments();
		int ValueReturnValueAndArguments(string param1, int param2);
		int ValueReturnValueAndNoArguments();
	}
}
