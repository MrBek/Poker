using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Poker.Client.Networking;
using Poker.Common.Networking;
using UnityEngine;

namespace Poker.Client.Behaviours
{
    public class PlayerNetworkBehaviour : MonoBehaviour,IPhotonPeerListener
    {
        private readonly Guid playerId = Guid.NewGuid();
        private readonly Dictionary<EventCodes,Action<EventData>> eventHandlers = new Dictionary<EventCodes, Action<EventData>>(); 

        private PhotonPeer peerSelf;

        public void Awake()
        {
            peerSelf = new PhotonPeer(this,ConnectionProtocol.Udp);
            peerSelf.Connect("localhost:5055", "Poker");
        }

        public void Update()
        {
            if (peerSelf != null)
            {
                peerSelf.Service();
            }
        }

        public void SendVoiceInput(byte[] data,int offset,int count)
        {
            if (peerSelf != null)
            {
                var sendBuffer = new byte[count];
                Buffer.BlockCopy(data, offset, sendBuffer, 0, count);
                peerSelf.Send(OperationCodes.VoiceInput, new[] {new KeyValuePair<byte, object>(1, sendBuffer),}, false);
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
           
        }

        public void OnEvent(EventData eventData)
        {
            Action<EventData> eventHandler = null;
            if ( eventHandlers.TryGetValue((EventCodes)eventData.Code,out eventHandler))
            {
                eventHandler(eventData);
            }
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
         
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            if ( statusCode == StatusCode.Connect )
            {
                peerSelf.Send(OperationCodes.PlayerIntroduction,new []{new KeyValuePair<byte, object>(1,playerId.ToByteArray())},true);
            }
        }

        public void OnDestroy()
        {
            if ( peerSelf != null )
            {
                peerSelf.Disconnect();
                peerSelf = null;
            }
        }

        private void ReceiveVoiceOutput(EventData eventData)
        {
            var data = eventData.Parameters[1] as byte[];

            var playerVoiceBehaviour = GetComponent<PlayerVoiceBehaviour>();

            if ( playerVoiceBehaviour != null )
            {
                playerVoiceBehaviour.PlayVoiceOutput(data,data.Length);
            }
        }

        private void ReceivePlayerAnnounce(EventData eventData)
        {
            var otherPlayerId = new Guid((byte[])eventData.Parameters[1]);

            if ( otherPlayerId == playerId )
            {
                Debug.Log("Self login announce: " + otherPlayerId);
            }
            else
            {
                Debug.Log("Other player login announce: " + otherPlayerId);
            }
        }

        private void RegisterHandler(EventCodes eventCode,Action<EventData> handler)
        {
            eventHandlers.Add(eventCode,handler);
        }

        public PlayerNetworkBehaviour()
        {
            RegisterHandler(EventCodes.VoiceOutput,ReceiveVoiceOutput);
            RegisterHandler(EventCodes.PlayerAnnounce, ReceivePlayerAnnounce);
        }
    }
}