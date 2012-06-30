using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Poker.Client.Networking;
using Poker.Common.Networking;
using Poker.Common.Networking.Messages;
using UnityEngine;

namespace Poker.Client.Behaviours
{
    public class PlayerNetworkBehaviour : NetworkBehaviourBase 
    {
        private readonly string playerName = Guid.NewGuid().ToString();
    
        public void Awake()
        {
            Connect("localhost:5055",playerName);
        }

        public void SendVoiceInput(byte[] data,int offset,int count)
        {
            var sendBuffer = new byte[count];
            Buffer.BlockCopy(data,offset,sendBuffer,0,count);
            SendToServer(new VoiceDataMessage() { Data = sendBuffer},false,1);
        }

        private void ReceiveVoiceOutput(VoiceDataMessage message)
        {
            var playerVoiceBehaviour = GetComponent<PlayerVoiceBehaviour>();

            if ( playerVoiceBehaviour != null )
            {
                playerVoiceBehaviour.PlayVoiceOutput(message.Data,message.Data.Length);
            }
        }

        public PlayerNetworkBehaviour()
        {
            RegisterHandler<VoiceDataMessage>(ReceiveVoiceOutput);
        }
    }
}