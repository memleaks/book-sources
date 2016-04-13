using KnownTypes.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace KnownTypes
{
	public static class MessageProcessorKnownTypesProvider
	{
		private static Type[] knownTypes;

		public static Type[] GetMessageTypes(
			ICustomAttributeProvider attributeTarget)
		{
			if(MessageProcessorKnownTypesProvider.knownTypes == null)
			{
				var types = new List<Type>();
				var messageType = typeof(Message);

				foreach(var type in 
					Assembly.GetAssembly(
						typeof(MessageProcessorKnownTypesProvider)).GetTypes())
				{
					if(messageType.IsAssignableFrom(type))
					{
						types.Add(type);
					}
				}

				MessageProcessorKnownTypesProvider.knownTypes = 
					types.ToArray();
			}

			return MessageProcessorKnownTypesProvider.knownTypes;
		}
	}
}
