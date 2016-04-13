using System;
using System.Runtime.Serialization;

namespace KnownTypes.Messages
{
	[DataContract]
	public class Message
	{
		[DataMember]
		public string Data;
		
		public Message()
			: base()
		{
			this.Data = "Unknown";
		}
	}
}
