using System;
using System.Collections.Generic;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using Poker.Common.Networking;

namespace Poker.Server
{
    public class PokerPeer : PeerBase
    {
        private static readonly ILogger                                                     log                 = LogManager.GetCurrentClassLogger();
        private readonly PokerApplication                                                   pokerApplication;
        private readonly Dictionary<OperationCodes,Action<OperationRequest,SendParameters>> operationHandlers   = new Dictionary<OperationCodes, Action<OperationRequest, SendParameters>>();

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            log.InfoFormat("Peer disconnected: {0}, {1}",reasonCode,reasonDetail);
            pokerApplication.Disconnect(this);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Action<OperationRequest, SendParameters> operationHandler = null;

            if ( operationHandlers.TryGetValue((OperationCodes)operationRequest.OperationCode,out operationHandler))
            {
                operationHandler(operationRequest, sendParameters);
            }
        }

        private void ReceiveVoiceInput(OperationRequest operationRequest,SendParameters sendParameters)
        {
            var data                    = operationRequest.Parameters[1] as byte[];
            var eventData               = new EventData((byte)EventCodes.VoiceOutput, new Dictionary<byte, object>() { { 1, operationRequest.Parameters[1] } });
            var outputSendParameters    = new SendParameters() {Unreliable = true};

            pokerApplication.Broadcast(eventData,outputSendParameters,p => true);
        }

        private void RegisterHandler(OperationCodes operationCode,Action<OperationRequest,SendParameters> handler)
        {
            operationHandlers.Add(operationCode,handler);
        }

        public PokerPeer(PokerApplication pokerApplication,IRpcProtocol protocol, IPhotonPeer unmanagedPeer) 
            : base(protocol, unmanagedPeer)
        {
            this.pokerApplication = pokerApplication;

            RegisterHandler(OperationCodes.VoiceInput, ReceiveVoiceInput);
        }
    }
}