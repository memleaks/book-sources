using KnownTypes.Contracts;
using KnownTypes.Messages;
using System;
using System.ServiceModel;

namespace KnownTypes.Services
{
	[ServiceBehavior]	
	public class MessageProcessor : IMessageProcessor
	{
		[OperationBehavior]
		public string Process(Message message)
		{
			return message.Data;
		}
	}
}
