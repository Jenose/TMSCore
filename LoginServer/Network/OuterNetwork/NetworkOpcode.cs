using LoginServer.Network.Packets.Ins;
using LoginServer.Network.Packets.Outs;
using System;
using System.Collections.Generic;

namespace LoginServer.OuterNetwork
{
    public class NetworkOpcode
    {
        public static Dictionary<short, Type> Recv = new Dictionary<short, Type>();
        public static Dictionary<Type, short> Send = new Dictionary<Type, short>();

        public static void Init()
        {
            Recv.Add(unchecked((short)0x0001), typeof(REQ_LOGIN));
            Recv.Add(unchecked((short)0x0019), typeof(REQ_RSAKEY));

            Send.Add(typeof(RES_LOGIN), unchecked((short)0x0000));
            Send.Add(typeof(RES_SERVERLIST), unchecked((short)0x0008));
            Send.Add(typeof(RES_RSAKEY), unchecked((short)0x0016));
            Send.Add(typeof(RES_LOGIN_WELCOME), unchecked((short)0x008B));
        }
    }
}
