using KnownTypes.Contracts;
using KnownTypes.Messages;
using KnownTypes.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ServiceModel;

namespace KnownTypes.Tests
{
	[TestClass]
	public sealed class MessageProcessorTests
	{
		private ServiceHost host;
		private IMessageProcessor channel;

		[TestInitialize]
		public void TestInitialize()
		{
			this.host = new ServiceHost(typeof(MessageProcessor));
			this.host.Open();
			this.channel = new ChannelFactory<IMessageProcessor>(string.Empty).CreateChannel();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			((IDisposable)this.channel).Dispose();
			this.host.Close();
		}
	
		[TestMethod]
		public void ProcessMessage()
		{
			Assert.AreEqual("Unknown", 
				this.channel.Process(new Message()));
		}

		[TestMethod]
		public void ProcessApplicationClosedMessage()
		{
			Assert.AreEqual("Application has closed.", 
				this.channel.Process(
					new ApplicationClosedMessage("\\SomeMachine")));
		}
	}
}
