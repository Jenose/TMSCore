using LoginServer.Database;
using LoginServer.Network.Packets.Outs;
using LoginServer.OuterNetwork;
using System;
using System.Security.Cryptography;
using System.Text;
using TMSCore.Models.Account;
using TMSCore.Utilities;

namespace LoginServer.Services
{
    public class AccountService
    {
        public static void AuthAccount(NetworkSession Session, string username, string password, string mac)
        {
            var account = MdbAccount.Instance.SelectAccountByName(username).Result;
            var hash_passwd = Convert.ToBase64String(new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(password+"*m4st3r")));

            if (account == null)
            {
                Account acc = new Account();
                acc.Id = MdbAccount.Instance.GetNextSequence("accountid");
                acc.Name = username;
                acc.Password = hash_passwd;
                acc.Salt = "*m4st3r";
                acc.SecondPassword = "";
                acc.SecondSalt = "*m4st3r2";
                acc.Loggedin = true;
                acc.LastLogin = DateTime.Now;
                acc.CreateDate = DateTime.Now;
                acc.Banned = false;
                acc.BanReason = string.Empty;
                acc.AccessLevel = 0;
                acc.IpAddress = "0.0.0.0";
                acc.MacAddress = mac;
                acc.Cash = 99999;
                acc.MaplePoint = 99999;
                acc.Gender = Gender.Male;
                MdbAccount.Instance.InsertAccount(acc);
                account = acc;
                //new RES_LOGIN(account, LoginResult.WrongPassword);
            }
            else if(!account.Password.Equals(hash_passwd))
            {
                new RES_LOGIN(account, LoginResult.WrongPassword).Send(Session);
                return;
            }
            else if(account.Banned)
            {
                new RES_LOGIN(account, LoginResult.Banned).Send(Session);
                return;
            }


            new RES_LOGIN(account, LoginResult.Success).Send(Session);
            new RES_LOGIN_WELCOME(null).Send(Session);
            new RES_SERVERLIST().Send(Session);
            new RES_SERVERLIST(true).Send(Session);
        }
    }
}
