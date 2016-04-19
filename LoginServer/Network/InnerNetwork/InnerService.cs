using Hik.Collections;
using Hik.Communication.ScsServices.Service;
using InnerLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using TMSCore.Utilities;

namespace LoginServer.InnerNetwork
{
    public class InnerService : ScsService, IInnerService
    {
        public static string SecurityKey = "0bacf71935dd3589a2c529d1cdaba6d0b80a82d673730fe752001f06a509a8ff";

        public readonly ThreadSafeSortedList<long, InnerClient> Authed = new ThreadSafeSortedList<long, InnerClient>();

        protected Queue<Action<InnerClient>> TaskPool = new Queue<Action<InnerClient>>();

        protected object TaskPoolLock = new object();

        public InnerService()
        {
            ThreadPool.QueueUserWorkItem(TaskInvoker);
        }
        
        protected void TaskInvoker(object o)
        {
            while (true)
            {
                Thread.Sleep(1);

                Action<InnerClient> action;

                lock (TaskPoolLock)
                {
                    if (TaskPool.Count < 1)
                        continue;

                    action = TaskPool.Dequeue();
                }

                foreach (var client in Authed.GetAllItems())
                {
                    try
                    {
                        action(client);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("InnerService:", ex);
                    }
                }
            }
        }

        public void Auth(string key, int serverId)
        {
            if (!key.Equals(SecurityKey))
            {
                CurrentClient.Disconnect();
                return;
            }

            Authed[CurrentClient.ClientId] =
                new InnerClient(
                    CurrentClient,
                    CurrentClient.GetClientProxy<IInnerClient>());

            CurrentClient.Disconnected += ClientDisconnected;
            Log.Info("InnerService: GameServer ID:{0} connected...", serverId);

            Authed[CurrentClient.ClientId].ServerId = serverId;
            Authed[CurrentClient.ClientId].Proxy.Authed();
        }

        private void ClientDisconnected(object sender, EventArgs e)
        {
            int gamesrvId = Authed[((IScsServiceClient)sender).ClientId].ServerId;
            Authed.Remove(((IScsServiceClient)sender).ClientId);
            Log.Info("InnerService: GameServer ID:{0} disconnected...", gamesrvId);
        }

        public void OnlineCount(int count)
        {
            Authed[CurrentClient.ClientId].OnlineCount = count;
        }
    }
}
