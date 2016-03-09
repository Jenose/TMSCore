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

namespace LoginServer
{
    class LoginServer
    {
        public static IInnerService InnerService;

        protected static IScsServiceApplication ServiceApplication;

        static void Main(string[] args)
        {
            InnerService = new InnerService();

            try
            {
                ServiceApplication = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(23232));
                ServiceApplication.AddService<IInnerService, InnerService>((InnerService)InnerService);
                ServiceApplication.Start();
                Log.Info("InnerService started at *:23232.");
            }
            catch (Exception ex)
            {
                Log.ErrorException("InnerService can not be started.", ex);
            }

            Process.GetCurrentProcess().WaitForExit();
        }
    }
}
