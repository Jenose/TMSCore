using LoginServer.OuterNetwork;
using System.IO;

namespace LoginServer.Network.Packets.Outs
{
    public class RES_SERVERLIST : ASendPacket
    {
        //protected ServerInfo ServerInfo;
        protected bool IsEndOfServerList;

        public RES_SERVERLIST(bool end = false) // RES_SERVERLIST(ServerInfo serverInfo)
        {
            //ServerInfo = serverInfo;
            IsEndOfServerList = end;
        }

        public override void Write(BinaryWriter writer)
        {
            if(!IsEndOfServerList)
            {
                WriteC(writer, 0); // server id
                WriteS(writer, "Test"); // World Name
                WriteC(writer, 0); // flag
                WriteS(writer, "Welcome to C# Server\r\nเซิฟเวอร์ที่จะฉีกทุกกฏของเมเปิ้ลสตอรี่");
                WriteH(writer, 100);
                WriteH(writer, 100);
                WriteC(writer, 1); // last channel channels.Count

                int i = 1;
                //foreach(var channel in channels)
                {
                    WriteS(writer, "Test" + "-" + i);
                    WriteD(writer, 1200); // user in channel
                    WriteC(writer, 0);
                    WriteH(writer, i - 1);
                }

                WriteH(writer, 100);
            }
            else
            {
                WriteD(writer, -1);
            }
            
        }
    }
}
