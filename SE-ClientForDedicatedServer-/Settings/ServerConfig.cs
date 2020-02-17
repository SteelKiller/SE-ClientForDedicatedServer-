using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SE_ClientForDedicatedServer_.Settings
{
    [XmlRoot("Settings")]
    public class ServerConfig
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string ApiKey { get; set; }
        public bool AutoConnect { get; set; }


        public ServerConfig()
        {

        }

        public ServerConfig(string Name)
        {
            this.Name = Name;
        }

        public ServerConfig(string Name, string IP, int Port, string ApiKey, bool AutoConnect)
        {
            this.Name = Name;
            this.Address = IP;
            this.Port = Port;
            this.ApiKey = ApiKey;
            this.AutoConnect = AutoConnect;
        }
    }
}
