using Hik.Communication.Scs.Communication.Messages;

namespace LoginServer.OuterNetwork
{
    public class NetworkMessage : ScsMessage
    {
        public short OpCode;

        public byte[] Data;
    }
}
