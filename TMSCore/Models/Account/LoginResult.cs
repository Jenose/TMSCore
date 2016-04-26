using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMSCore.Models.Account
{
    public enum LoginResult : byte
    {
        Success = 0,
        Banned = 3,
        WrongPassword = 4,
        SystemError = 6,
        AlreadyLogin = 7,
        AccountNotActive = 16,
        IpBanned = 32
    }
}
