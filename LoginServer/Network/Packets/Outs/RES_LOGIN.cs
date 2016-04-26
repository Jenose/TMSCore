using LoginServer.OuterNetwork;
using System.IO;
using TMSCore.Models.Account;
using TMSCore.Utilities;

namespace LoginServer.Network.Packets.Outs
{
    public class RES_LOGIN : ASendPacket
    {
        protected Account Account;
        protected LoginResult Result;

        public RES_LOGIN(Account account, LoginResult result)
        {
            Account = account;
            Result = result;
        }

        public override void Write(BinaryWriter writer)
        {
            Log.Debug("LoginResult = {0}", Result);

            if(Result == LoginResult.Success)
            {
                WriteC(writer, (byte)Result);
                WriteD(writer, Account.Id);
                WriteC(writer, (byte)Account.Gender);
                WriteC(writer, (byte)((Account.IsGM()) ? 1 : 0)); // Admin byte - Find, Trade, etc.
                WriteC(writer, (byte)((Account.IsGM()) ? 1 : 0)); // Admin byte - Commands
                WriteH(writer, 0);
                WriteD(writer, 0); //0 for new accounts
                WriteB(writer, new byte[6]);
                WriteC(writer, 1);
                WriteH(writer, 0);
            }
            else
            {
                WriteC(writer, (byte)Result);
                if(Result == LoginResult.AlreadyLogin)
                    WriteB(writer, new byte[5]);
            }
        }
    }
}
