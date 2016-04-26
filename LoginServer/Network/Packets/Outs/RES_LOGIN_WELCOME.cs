using LoginServer.OuterNetwork;
using System.Collections.Generic;
using System.IO;

namespace LoginServer.Network.Packets.Outs
{
    public class RES_LOGIN_WELCOME : ASendPacket
    {
        protected Dictionary<string, int> Flags;

        public RES_LOGIN_WELCOME(Dictionary<string, int> flags)
        {
            Flags = flags;
        }

        public override void Write(BinaryWriter writer)
        {
            WriteC(writer, (Flags == null) ? 0 : Flags.Count);
            if (Flags != null)
            {
                foreach(var f in Flags)
                {
                    WriteS(writer, f.Key);
                    WriteC(writer, f.Value);
                }
            }
        }
    }
}
