using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Poker.Common.Networking;
using Poker.Common.Networking.Messages;
using UnityEngine;

namespace Poker.Client.Behaviours
{
    public class PlayerNetworkBehaviour : PlayerNetworkBehaviourBase 
    {
        private readonly string playerName = Guid.NewGuid().ToString();
    
        public void Awake()
        {
            Connect("localhost:5055",playerName);
        }

        public void SendVoiceInput(byte[] data,int offset,int count,int sampleFrequency)
        {
            var sendBuffer = new byte[count];
            Buffer.BlockCopy(data,offset,sendBuffer,0,count);
            SendToServer(new VoiceDataMessage() { Data = sendBuffer,SampleFrequency = sampleFrequency},false,1);
        }

        private void ReceiveVoiceData(VoiceDataMessage message)
        {
            var playerVoiceBehaviour = FindPlayerByName<PlayerVoiceOutputBehaviour>(playerName);

            if ( playerVoiceBehaviour != null )
            {
                playerVoiceBehaviour.PlayVoiceOutput(message.Data,message.Data.Length,message.SampleFrequency);
            }
        }

        public PlayerNetworkBehaviour()
        {
            RegisterHandler<VoiceDataMessage>(ReceiveVoiceData);
        }
    }
}