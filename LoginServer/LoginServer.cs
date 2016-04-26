using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using InnerLib.Interfaces;
using LoginServer.InnerNetwork;
using LoginServer.OuterNetwork;
using System;
using System.Diagnostics;
using TMSCore.Utilities;

namespace LoginServer
{
    class LoginServer
    {
        /// <summary>
        /// 
        /// </summary>
        public static IInnerService InnerService;

        /// <summary>
        /// 
        /// </summary>
        public static NetworkListener m_NetworkListener;

        /// <summary>
        /// 
        /// </summary>
        protected static IScsServiceApplication ServiceApplication;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
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

            m_NetworkListener = new NetworkListener("*", 8484, 1000);
            m_NetworkListener.BeginListening();

            Process.GetCurrentProcess().WaitForExit();
        }
    }
}
