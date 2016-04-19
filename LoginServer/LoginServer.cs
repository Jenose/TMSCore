using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using LoginServer.InnerNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InnerLib.Interfaces;
using TMSCore.Utilities;
using LoginServer.OuterNetwork;

namespace LoginServer
{
    class LoginServer
    {
        public static IInnerService InnerService;

        /// <summary>
        /// 
        /// </summary>
        public static NetworkListener m_NetworkListener;

        protected static IScsServiceApplication ServiceApplication;

        static void Main(string[] args)
        {
            InnerService = new InnerService();

            try
            {
                ServiceApplication = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(7474));
                ServiceApplication.AddService<IInnerService, InnerService>((InnerService)InnerService);
                ServiceApplication.Start();
                Log.Info("InnerService started at *:7474.");
            }
            catch (Exception ex)
            {
                Log.ErrorException("InnerService can not be started.", ex);
            }

            NetworkOpcode.Init();

            m_NetworkListener = new NetworkListener("127.0.0.1", 8484, 1000);
            m_NetworkListener.BeginListening();

            Process.GetCurrentProcess().WaitForExit();
        }
    }
}
