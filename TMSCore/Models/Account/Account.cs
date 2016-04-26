using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TMSCore.Models.Account
{
    public class Account
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SecondPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SecondSalt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Loggedin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastLogin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Banned { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BanReason { get; set; }

        /// <summary>
        /// 0 = user, 1 = vip, 2 = premium, 3 = GM, 4 = Admin
        /// </summary>
        public int AccessLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Cash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaplePoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsGM()
        {
            return AccessLevel >= 3;
        }
    }
}
