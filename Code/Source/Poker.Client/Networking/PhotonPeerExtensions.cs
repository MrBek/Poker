using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Poker.Common.Networking;

namespace Poker.Client.Networking
{
    public static class PhotonPeerExtensions
    {
        public static void Send(this PhotonPeer peer,OperationCodes code,IEnumerable<KeyValuePair<byte,object>> parameters,bool reliable)
        {
            peer.OpCustom((byte) code, parameters.ToDictionary(kv => kv.Key, kv => kv.Value), reliable);
        }
    }
}