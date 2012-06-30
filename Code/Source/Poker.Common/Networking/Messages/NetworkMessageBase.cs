using System.IO;

namespace Poker.Common.Networking.Messages
{
    public abstract class NetworkMessageBase : INetworkMessage 
    {
        public string PlayerName { get; set; }

        public void Read(BinaryReader reader)
        {
            PlayerName = reader.ReadString();
            OnRead(reader);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(PlayerName);
            OnWrite(writer);
        }

        protected abstract void OnRead(BinaryReader reader);
        protected abstract void OnWrite(BinaryWriter writer);   
    }
}