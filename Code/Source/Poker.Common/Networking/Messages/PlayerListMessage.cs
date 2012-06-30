using System.Collections.Generic;
using System.Linq;

namespace Poker.Common.Networking.Messages
{
    public class PlayerListMessage : NetworkMessageBase 
    {
        public IEnumerable<string> PlayerNames { get; set; }

        protected override void OnRead(System.IO.BinaryReader reader)
        {
            var playerNameCount = reader.ReadInt32();
            var playerNames     = new List<string>();

            for( var i = 0; i < playerNameCount; ++i )
            {
                playerNames.Add(reader.ReadString());
            }

            PlayerNames = playerNames;
        }

        protected override void OnWrite(System.IO.BinaryWriter writer)
        {
            writer.Write(PlayerNames.Count());
            foreach( var playerName in PlayerNames)
            {
                writer.Write(playerName);
            }
        }
    }
}