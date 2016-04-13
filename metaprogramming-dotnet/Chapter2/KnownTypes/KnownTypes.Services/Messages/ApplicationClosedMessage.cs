using System;
using System.Runtime.Serialization;

namespace KnownTypes.Messages
{
	[DataContract]
	public sealed class ApplicationClosedMessage : Message
	{
		[DataMember]
		public string MachineName;

		public ApplicationClosedMessage(string machineName)
			: base()
		{
			this.MachineName = machineName;
			this.Data = "Application has closed.";
		}
	}
}
