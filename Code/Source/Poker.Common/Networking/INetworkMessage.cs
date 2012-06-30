using System.IO;

namespace Poker.Common.Networking
{
    public interface INetworkMessage
    {
        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}