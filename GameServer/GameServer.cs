using GameServer.InnerNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class GameServer
    {
        public static InnerConnection InnerConnection = new InnerConnection();

        static void Main(string[] args)
        {
            InnerConnection.Connect();

            Process.GetCurrentProcess().WaitForExit();
        }
    }
}
