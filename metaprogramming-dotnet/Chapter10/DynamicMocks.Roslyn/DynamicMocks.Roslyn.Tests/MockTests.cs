using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicMocks.Roslyn.Tests
{
	[TestClass]
	public sealed class MockTests
	{
		[TestMethod]
		public void Create()
		{
			var callback = new SimpleCallback();
			var mock = Mock.Create<ISimpleInterface>(callback);

			Assert.IsNotNull(mock);
		}

		[TestMethod]
		public void RecordCall()
		{
			var callback = new SimpleCallback();
			var mock = Mock.Create<ISimpleInterface>(callback);
			mock.NoReturnValueAndNoArguments();
			var callbacks = callback.GetCallbacks();
			Assert.AreEqual(1, callbacks.Count);
			Assert.IsTrue(callbacks.Contains("NoReturnValueAndNoArguments"));
		}

		[TestMethod]
		public void RecordCallWithNoCallback()
		{
			var callback = new SimpleCallbackWithNoMethods();
			var mock = Mock.Create<ISimpleInterface>(callback);
			mock.NoReturnValueAndNoArguments();
			var callbacks = callback.GetCallbacks();
			Assert.AreEqual(0, callbacks.Count);
		}

		[TestMethod]
		public void UseTestCallback()
		{
			var callback = new TestCallback();
			var mock = Mock.Create<ITest>(callback);
			var result = mock.CallMe();
			this.TestContext.WriteLine(result.ToString());
		}

		public TestContext TestContext { get; set; }
	}
}
