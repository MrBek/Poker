using System;
using System.Collections.Generic;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using Poker.Common.Networking;
using Poker.Common.Networking.Messages;
using Poker.Server.Networking;

namespace Poker.Server
{
    public class PokerPeer : NetworkPeerBase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
           
        private void ReceiveVoiceData(VoiceDataMessage message)
        {
            BroadcastToClients(message,false,1);   
        }   
       
        public PokerPeer(PokerApplication pokerApplication,IRpcProtocol protocol, IPhotonPeer unmanagedPeer) 
            : base(pokerApplication,protocol, unmanagedPeer)
        {
            RegisterHandler<VoiceDataMessage>(ReceiveVoiceData);         
        }
    }
}