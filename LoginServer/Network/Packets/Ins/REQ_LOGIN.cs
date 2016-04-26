using LoginServer.OuterNetwork;
using LoginServer.Services;
using System;

namespace LoginServer.Network.Packets.Ins
{
    public class REQ_LOGIN : ARecvPacket
    {
        protected string Name;
        protected string Password;
        protected string MacAddress;

        public override void Read()
        {
            Name = ReadS();
            Password = ReadS();
            MacAddress = BitConverter.ToString(ReadB(6));
        }

        public override void Process()
        {
            AccountService.AuthAccount(Session, Name, Password, MacAddress);
        }
    }
}
