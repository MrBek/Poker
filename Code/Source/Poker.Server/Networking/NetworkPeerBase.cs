using System;
using System.Collections.Generic;
using System.Linq;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using Poker.Common.Networking;
using Poker.Common.Networking.Messages;

namespace Poker.Server.Networking
{
    public class NetworkPeerBase : PeerBase
    {
        private static readonly List<NetworkPeerBase>                       allPeers        = new List<NetworkPeerBase>(); 
        private readonly        PokerApplication                            application;
        private readonly        NetworkMessageSerializer                    serializer      = new NetworkMessageSerializer();
        private readonly        Dictionary<Type,Action<INetworkMessage>>    messageHandlers = new Dictionary<Type, Action<INetworkMessage>>(); 

        public string PlayerName { get; private set; }

        public static IEnumerable<string> AllPlayerNames
        {
            get
            {
                lock(allPeers)
                {
                    return allPeers.Select(p => p.PlayerName).ToArray();
                }
            }
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            lock(allPeers)
            {
                allPeers.Remove(this);
            }

            BroadcastToClients(new PlayerListMessage() { PlayerName = this.PlayerName,PlayerNames = AllPlayerNames});
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var message = serializer.FromMessageData(operationRequest.OperationCode, operationRequest.Parameters);

            if ( message != null )
            {
                Action<INetworkMessage> handler = null;
                if ( messageHandlers.TryGetValue(message.GetType(),out handler))
                {
                    handler(message);
                }
            }
        }

        protected void BroadcastToClients(INetworkMessage message)
        {
            BroadcastToClients(message, true);
        }

        protected void BroadcastToClients(INetworkMessage message,bool reliable)
        {
            BroadcastToClients(message, reliable, 0);
        }

        protected void BroadcastToClients(INetworkMessage message,bool reliable,byte channel)
        {
            var messageData = SerializeMessage(message, reliable, channel);

            if (messageData.Key != null)
            {
                IEnumerable<NetworkPeerBase> peerListCopy = null;

                lock (allPeers)
                {
                    peerListCopy = allPeers.ToArray();
                }

                application.BroadCastEvent(messageData.Key,peerListCopy,messageData.Value);
            }
        }

        protected void SendResponse(INetworkMessage message,bool reliable)
        {
            SendResponse(message, reliable, 0);
        }

        protected void SendResponse(INetworkMessage message,byte channel)
        {
            SendResponse(message, true, channel);
        }

        protected void SendResponse(INetworkMessage message,bool reliable,byte channel)
        {
            var messageData = SerializeMessage(message, reliable, channel);
            
            if ( messageData.Key != null )
            {
                SendOperationResponse(new OperationResponse(messageData.Key.Code, messageData.Key.Parameters), messageData.Value);
            }
        }

        protected void SendToClient(INetworkMessage message)
        {
            SendToClient(message, true);
        }

        protected void SendToClient(INetworkMessage message,bool reliable)
        {
            SendToClient(message, reliable, 0);
        }

        protected void SendToClient(INetworkMessage message,bool reliable,byte channel)
        {
            SendToClient(null, message, reliable, channel);
        }

        protected void SendToClient(NetworkPeerBase target,INetworkMessage message)
        {
            SendToClient(target, message, true);
        }

        protected void SendToClient(NetworkPeerBase target,INetworkMessage message,bool reliable)
        {
            SendToClient(target, message, reliable, 0);
        }

        protected void SendToClient(NetworkPeerBase target,INetworkMessage message,bool reliable,byte channel)
        {
            var messageData = SerializeMessage(message, reliable, channel);

            if ( messageData.Key != null )
            {
                target.SendEvent(messageData.Key, messageData.Value);
            }
        }

        protected void RegisterHandler<TMessage>(Action<TMessage> messageHandler)
            where TMessage : INetworkMessage 
        {
            messageHandlers.Add(typeof(TMessage),message => messageHandler((TMessage)message));
        }

        private KeyValuePair<EventData,SendParameters> SerializeMessage(INetworkMessage message,bool reliable,byte channel)
        {
            var messageData = serializer.ToMessageData(message);

            if (messageData.Value != null)
            {
                return new KeyValuePair<EventData, SendParameters>(new EventData(messageData.Key,messageData.Value),new SendParameters() { Unreliable = !reliable,ChannelId = channel});
            }

            return new KeyValuePair<EventData, SendParameters>(null,new SendParameters());
        }

        private void ReceivePlayerAnnounce(PlayerAnnounceMessage message)
        {
            PlayerName = message.PlayerName;

            BroadcastToClients(new PlayerListMessage() { PlayerName = PlayerName,PlayerNames = AllPlayerNames});
        }

        public NetworkPeerBase(PokerApplication application,IRpcProtocol protocol, IPhotonPeer unmanagedPeer)
            : base(protocol, unmanagedPeer)
        {
            this.application = application;

            lock(allPeers)
            {
                allPeers.Add(this);
            }

            RegisterHandler<PlayerAnnounceMessage>(ReceivePlayerAnnounce);
        }
    }
}