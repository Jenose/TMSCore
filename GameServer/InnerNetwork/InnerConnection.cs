using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using InnerLib.Interfaces;
using System;
using System.Threading;
using TMSCore.Utilities;

namespace GameServer.InnerNetwork
{
    /// <summary>
    /// GameServer Communicate with LoginServer Class
    /// </summary>
    public class InnerConnection
    {
        /// <summary>
        /// 
        /// </summary>
        public InnerClient Client = null;

        /// <summary>
        /// 
        /// </summary>
        public IScsServiceClient<IInnerService> ScsClient = null;

        /// <summary>
        /// 
        /// </summary>
        public InnerConnection()
        {
            Client = new InnerClient();
            Client.OnAuthed += OnAuthed;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnAuthed()
        {
            Log.Info("Authed with LoginServer");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Connect()
        {
            if (ScsClient != null)
                ScsClient.Disconnect();

            ScsClient = ScsServiceClientBuilder.CreateClient<IInnerService>(new ScsTcpEndPoint("127.0.0.1", 23232), Client);

            ScsClient.Timeout = 1000;
            ScsClient.ConnectTimeout = 2000;

            ScsClient.Connected += OnScsConnnected;
            ScsClient.Disconnected += OnScsDisconnected;

            Log.Trace("Try connect...");

            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    try
                    {
                        ScsClient.Connect();
                    }
                    catch
                    {
                        Log.Error("Can't connect to server.");
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScsDisconnected(object sender, EventArgs e)
        {
            Log.Info("Disconnected.");
            ScsClient = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScsConnnected(object sender, EventArgs e)
        {
            Log.Info("Connected.");
            Log.Info("Try auth...");

            ScsClient.ServiceProxy.Auth("0bacf71935dd3589a2c529d1cdaba6d0b80a82d673730fe752001f06a509a8ff", 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public void SendUserOnlineCount(int count)
        {
            ScsClient.ServiceProxy.OnlineCount(count);
        }

    }
}
