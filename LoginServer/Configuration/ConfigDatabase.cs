using Nini.Config;
using System.IO;

namespace LoginServer.Configuration
{
    public class ConfigDatabase : ConfigBase
    {
        /// <summary>
        /// 
        /// </summary>
        private string m_Path = Path.GetFullPath("Config/Database.ini");

        /// <summary>
        /// 
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        public ConfigDatabase()
        {
            source = new IniConfigSource(m_Path);
            Host = source.Configs["database"].GetString("host");
            Name = source.Configs["database"].GetString("name");
        }
    }
}
