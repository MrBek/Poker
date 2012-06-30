using System;
using System.Collections.Generic;
using System.IO;
using Poker.Common.Networking.Messages;

namespace Poker.Common.Networking
{
    public class NetworkMessageSerializer
    {
        private readonly Dictionary<byte,Func<INetworkMessage>> messageFactories    = new Dictionary<byte, Func<INetworkMessage>>();
        private readonly Dictionary<Type,byte>                  messageTypes        = new Dictionary<Type, byte>(); 

        public INetworkMessage FromMessageData(byte code,IDictionary<byte,object> values)
        {
            Func<INetworkMessage> factory = null;

            if ( messageFactories.TryGetValue(code,out factory))
            {
                var message     = factory();
                object messageDataObject = null;

                if ( values.TryGetValue(0,out messageDataObject))
                {
                    var messageData = messageDataObject as byte[];
                    if ( messageData != null )
                    {
                        using( var memoryStream = new MemoryStream(messageData))
                        {
                            var binaryReader = new BinaryReader(memoryStream);
                            message.Read(binaryReader);

                            return message;
                        }
                    }
                }
            }

            return null;
        }

        public KeyValuePair<byte,Dictionary<byte,object>> ToMessageData(INetworkMessage message)
        {
            byte messageCode = 0;
            if ( messageTypes.TryGetValue(message.GetType(),out messageCode))
            {
                using( var memoryStream = new MemoryStream())
                {
                    var binaryWriter = new BinaryWriter(memoryStream);
                    message.Write(binaryWriter);
                    
                    binaryWriter.Flush();
                    memoryStream.Flush();

                    var messageData = new Dictionary<byte, object>()
                    {
                        {0, memoryStream.ToArray()}
                    };

                    return new KeyValuePair<byte, Dictionary<byte, object>>(messageCode,messageData);
                }
            }

            return new KeyValuePair<byte, Dictionary<byte, object>>(byte.MaxValue,null);
        }

        public void RegisterMessageType<TMessage>(byte messageCode)
            where TMessage : INetworkMessage,new()
        {
            messageFactories.Add(messageCode,() => new TMessage());
            messageTypes.Add(typeof(TMessage),messageCode);
        }

        public NetworkMessageSerializer()
        {
            RegisterMessageType<PlayerAnnounceMessage>(10);
            RegisterMessageType<VoiceDataMessage>(100);
        }
    }
}