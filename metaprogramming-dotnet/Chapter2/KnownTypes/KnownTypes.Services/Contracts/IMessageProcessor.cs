using KnownTypes.Messages;
using System;
using System.ServiceModel;

namespace KnownTypes.Contracts
{
	[ServiceContract]
	//[ServiceKnownType("GetMessageTypes",
	//   typeof(MessageProcessorKnownTypesProvider))]
	public interface IMessageProcessor
	{
		[OperationContract]
		string Process(Message fruit);
	}
}
