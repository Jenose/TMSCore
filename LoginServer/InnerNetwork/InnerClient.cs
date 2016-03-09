using Hik.Communication.ScsServices.Service;
using InnerLib.Interfaces;
using System.Collections.Generic;

namespace LoginServer.InnerNetwork
{
    /// <summary>
    /// GameServer
    /// </summary>
    public class InnerClient
    {
        /// <summary>
        /// 
        /// </summary>
        public IScsServiceClient Client { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IInnerClient Proxy { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OnlineCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="proxy"></param>
        public InnerClient(IScsServiceClient client, IInnerClient proxy)
        {
            Client = client;
            Proxy = proxy;
        }
    }
}
