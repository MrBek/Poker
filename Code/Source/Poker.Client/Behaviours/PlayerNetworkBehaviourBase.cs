using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Poker.Common.Networking;
using Poker.Common.Networking.Messages;
using UnityEngine;

namespace Poker.Client.Behaviours
{
    public abstract class PlayerNetworkBehaviourBase : MonoBehaviour,IPhotonPeerListener
    {
        private PhotonPeer                                                  selfPeer;
        private string                                                      playerName;
        private readonly NetworkMessageSerializer                           serializer      = new NetworkMessageSerializer();
        private readonly Dictionary<Type, Action<INetworkMessage,bool>>     messageHandlers = new Dictionary<Type, Action<INetworkMessage,bool>>();
        private readonly List<string>                                       allPlayerNames  = new List<string>(); 
        
        public IEnumerable<string> AllPlayerNames
        {
            get { return allPlayerNames; }
        }

        public void Update()
        {
            if ( selfPeer != null )
            {
                selfPeer.Service();
            }

            OnUpdate();
        }

        public void OnDestroy()
        {
            if ( selfPeer != null )
            {
                selfPeer.Disconnect();
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
           
        }

        public void OnEvent(EventData eventData)
        {
            HandleMessage(eventData.Code,eventData.Parameters,false);
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            HandleMessage(operationResponse.OperationCode,operationResponse.Parameters,true);
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            if ( statusCode == StatusCode.Connect )
            {
                SendToServer(new PlayerAnnounceMessage());
            }
        }

        protected virtual void OnUpdate()
        {}

        protected void Connect(string address,string playerName)
        {
            this.playerName = playerName;

            selfPeer = new PhotonPeer(this, ConnectionProtocol.Udp);
            selfPeer.Connect(address, "Poker");
        }

        protected void SendToServer(INetworkMessage message)
        {
            SendToServer(message, true);
        }

        protected void SendToServer(INetworkMessage message, bool reliable)
        {
            SendToServer(message, reliable, 0);
        }

        protected void SendToServer(INetworkMessage message,bool reliable,byte channel)
        {
            var baseMessage = message as NetworkMessageBase;

            if ( baseMessage != null )
            {
                baseMessage.PlayerName = playerName;
            }

            if ( selfPeer != null )
            {
                var messageData = serializer.ToMessageData(message);

                if ( messageData.Value != null )
                {
                    selfPeer.OpCustom(messageData.Key, messageData.Value, reliable, channel);
                }
            }
        }

        protected void RegisterHandler<TMessage>(Action<TMessage> messageHandler)
            where TMessage : INetworkMessage
        {
            RegisterHandler<TMessage>((message,isResponse) => messageHandler(message));
        }

        protected void RegisterHandler<TMessage>(Action<TMessage,bool> messageHandler)
            where TMessage : INetworkMessage 
        {
            messageHandlers.Add(typeof(TMessage),(message,isResponse) => messageHandler((TMessage)message,isResponse));
        }

        private void HandleMessage(byte messageCode,IDictionary<byte,object> values,bool isResponse)
        {
            var message = serializer.FromMessageData(messageCode, values);

            if ( message != null )
            {
                Action<INetworkMessage, bool> handler = null;
                if ( messageHandlers.TryGetValue(message.GetType(),out handler))
                {
                    handler(message, isResponse);
                }
            }
        }

        protected TComponent FindPlayerByName<TComponent>(string playerName)
            where TComponent : Component
        {
            var index = allPlayerNames.IndexOf(playerName);
            if (index >= 0)
            {
                var foundGameObject = GameObject.Find("Player0" + (index +1));

                if (foundGameObject != null)
                {  
                    return foundGameObject.GetComponent<TComponent>();
                }
            }

            return null;
        }

        private void ReceivePlayerList(PlayerListMessage message)
        {
            Debug.Log("Player list");
            allPlayerNames.Clear();
            allPlayerNames.AddRange(message.PlayerNames);
            allPlayerNames.Sort();

            foreach( var playerName in allPlayerNames )
            {
                var playerObject = FindPlayerByName<PlayerVoiceOutputBehaviour>(playerName).gameObject;
                
                for( var i = 0; i < playerObject.transform.childCount; ++i )
                {
                    var labelObject = playerObject.transform.GetChild(i);

                    if ( labelObject != null && labelObject.name == "Label")
                    {
                        labelObject.GetComponent<TextMesh>().text = playerName;
                    }
                }
            }
        }

        protected PlayerNetworkBehaviourBase()
        {
            RegisterHandler<PlayerListMessage>(ReceivePlayerList);
        }
    }
}