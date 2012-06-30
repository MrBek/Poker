using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Poker.Common.Networking;
using Poker.Server.Networking;
using log4net.Config;

namespace Poker.Server
{
    public class PokerApplication : ApplicationBase
    {
        private static readonly ILogger     log         = LogManager.GetCurrentClassLogger();
  
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var result = new PokerPeer(this,initRequest.Protocol,initRequest.PhotonPeer);
            return result;
        }

        protected override void Setup()
        {
            var path = Path.Combine(this.BinaryPath, "log4net.config");
            var file = new FileInfo(path);

            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }

            log.InfoFormat("Created application Instance: type={0}", Instance.GetType());
            Protocol.AllowRawCustomValues = true;
        }

        protected override void TearDown()
        {
           
        }
    }
}