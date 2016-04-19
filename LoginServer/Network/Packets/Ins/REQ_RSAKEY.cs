using LoginServer.Network.Packets.Outs;
using LoginServer.OuterNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Network.Packets.Ins
{
    public class REQ_RSAKEY : ARecvPacket
    {
        public override void Read()
        {
            
        }

        public override void Process()
        {
            new RES_RSAKEY().Send(Session);
        }
    }
}
