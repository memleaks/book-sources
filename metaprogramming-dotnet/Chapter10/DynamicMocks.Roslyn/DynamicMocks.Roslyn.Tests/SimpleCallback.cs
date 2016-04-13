
namespace DynamicMocks.Roslyn.Tests
{
	public sealed class SimpleCallback
		: Recorder
	{
		public SimpleCallback()
			: base() { }

		public void NoReturnValueAndArguments(string param1, int param2)
		{
			this.Callbacks.Add("NoReturnValueAndArguments");
		}

		public void NoReturnValueAndNoArguments()
		{
			this.Callbacks.Add("NoReturnValueAndNoArguments");
		}

		public string ReferenceReturnValueAndArguments(string param1, int param2)
		{
			this.Callbacks.Add("ReferenceReturnValueAndArguments");
			return null;
		}

		public string ReferenceReturnValueAndNoArguments()
		{
			this.Callbacks.Add("ReferenceReturnValueAndNoArguments");
			return null;
		}

		public int ValueReturnValueAndArguments(string param1, int param2)
		{
			this.Callbacks.Add("ValueReturnValueAndArguments");
			return default(int);
		}

		public int ValueReturnValueAndNoArguments()
		{
			this.Callbacks.Add("ValueReturnValueAndNoArguments");
			return default(int);
		}
	}
}
