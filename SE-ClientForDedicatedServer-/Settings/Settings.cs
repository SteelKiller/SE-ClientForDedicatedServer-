using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SE_ClientForDedicatedServer_.Settings
{
    [XmlRootAttribute("Settings", Namespace = "http://www.cpandl.com",
IsNullable = false)]
    public class Settings
    {
        private static Settings _setting;

        private static string FilePath = "Settings.xml";
        public static void AddServer(ServerConfig serverConfig)
        {
            if (_setting == null)
                _setting = new Settings();

            if (serverConfig.AutoConnect)
            {
                _setting.AutoStart = serverConfig;
                for (int i = 0; i < _setting.servers.Count; i++)
                {
                    _setting.servers[i].AutoConnect = false;                   
                }
            }

            _setting.servers.Add(serverConfig);
            SaveSettings();
        }
        public static ServerConfig ServerForAutoStart()
        {
            if (_setting == null || _setting.AutoStart == null)
                return null;
            return _setting.AutoStart;
        }
        public static void DeleteServer(ServerConfig serverConfig)
        {
            if (_setting == null)
                return;
            _setting.servers.Remove(serverConfig);
        }
        public static ObservableCollection<ServerConfig> GetServerList()
        {
            if (_setting == null)
            {
                _setting = new Settings();
                _setting.servers.Add(new ServerConfig("New Connection"));
            }
            return new ObservableCollection<ServerConfig>(_setting.servers);
        }
        private static void SaveSettings()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
            TextWriter textWriter = new StreamWriter(FilePath);
            xmlSerializer.Serialize(textWriter, _setting);
            textWriter.Close();
        }
        public static void LoadSettings()
        {
            if (!File.Exists(FilePath))
                return;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
            TextReader textReader = new StreamReader(FilePath);
            _setting = (Settings)xmlSerializer.Deserialize(textReader);
            textReader.Close();
        }
        public static string DumpGPS => _setting.GPSDumpPosition;




        [XmlArray("ServerConfigList"), XmlArrayItem(typeof(ServerConfig), ElementName = "ServerConfig")]
        public List<ServerConfig> servers;
        public ServerConfig AutoStart;
        public string GPSDumpPosition;
        public int DumpRadius;
        public byte LoginTimeControlBy;
        public byte MoveGridsToDump;



        public Settings()
        {
            servers = new List<ServerConfig>();            
        }

    }
}
