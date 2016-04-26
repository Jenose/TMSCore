using LoginServer.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TMSCore.Models.Account;

/// <summary>
/// Owner: Mellowz
/// </summary>
namespace LoginServer.Database
{
    public class MdbAccount
    {
        /// <summary>
        /// 
        /// </summary>
        private static MdbAccount m_Instance;

        /// <summary>
        /// 
        /// </summary>
        protected static IMongoClient _client;
        /// <summary>
        /// 
        /// </summary>
        protected static IMongoDatabase _database;

        /// <summary>
        /// ชื่อตารางเทเปิ้ลที่ใช้งานของ class นี้
        /// </summary>
        protected string table = "accounts";

        /// <summary>
        /// 
        /// </summary>
        public MdbAccount()
        {
            _client = new MongoClient(ConfigManager.Database.Host);
            _database = _client.GetDatabase(ConfigManager.Database.Name);
        }

        /// <summary>
        /// อินเสิร์ทแอ๊คเค้าท์ เพิ่มข้อมูลเข้าฐานข้อมูล
        /// </summary>
        /// <param name="account"></param>
        public bool InsertAccount(Account account)
        {
            var collection = _database.GetCollection<Account>(table);
            var result = collection.InsertOneAsync(account);
            return (result != null) ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Account> SelectAccountById(ObjectId id)
        {
            var collection = _database.GetCollection<Account>(table);
            var filter = Builders<Account>.Filter.Eq("_id", id);
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Account> SelectAccountByName(string name)
        {
            var collection = _database.GetCollection<Account>(table);
            var filter = Builders<Account>.Filter.Eq("Name", name);
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAccount(Account account)
        {
            var collection = _database.GetCollection<Account>(table);
            var filter = Builders<Account>.Filter.Eq("Id", account.Id);
            var result = await collection.ReplaceOneAsync(filter, account);
            return (result != null) ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetNextSequence(string name)
        {
            var collection = _database.GetCollection<BsonDocument>("counters");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", name);
            var result = collection.Find(filter);
            int id = Convert.ToInt32(result.First().GetValue("sequence_value"));
            var update = Builders<BsonDocument>.Update.Set("sequence_value", id+1);
            collection.UpdateOne(filter, update);
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        public static MdbAccount Instance
        {
            get { return (m_Instance != null) ? m_Instance : m_Instance = new MdbAccount(); }
        }
    }
}
