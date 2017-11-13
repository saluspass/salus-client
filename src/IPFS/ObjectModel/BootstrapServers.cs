using Ipfs.Api;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Salus
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class BootstrapServers
    {
        #region Constants

        public const string ServerAddress = "https://raw.githubusercontent.com/saluspass/salus-client/master/bootstrap.json";
        public static readonly string BootstrapLocalFile = Path.Combine(FileSystemConstants.PswmgrConfigFolder, "bootstrap.json");

        #endregion

        #region Variables

        private static BootstrapServers _Instance;

        private List<Peer> _Servers;

        #endregion

        #region Ctor

        private BootstrapServers()
        {
            _Servers = new List<Peer>();
        }

        #endregion

        #region Properties

        [JsonProperty]
        public List<Peer> Servers
        {
            get { return _Servers; }
            set { _Servers = value; }
        }

        public static async Task<BootstrapServers> Instance()
        {
            if(_Instance == null)
            {
                _Instance = await Load();
            }
            return  _Instance;
        }

        #endregion

        #region Methods

        private static async Task<BootstrapServers> Load()
        {
            BootstrapServers returnValue = null;
            try
            {
                byte[] downloadedData;
                using (WebClient webClient = new WebClient())
                {
                    downloadedData = await webClient.DownloadDataTaskAsync(ServerAddress);
                }

                string json = Encoding.UTF8.GetString(downloadedData);
                returnValue = JsonConvert.DeserializeObject<BootstrapServers>(json);
            }
            catch
            {
                returnValue = new BootstrapServers();
            }

            returnValue.Save();

            return returnValue;
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(BootstrapLocalFile))
            {
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public async Task Install()
        {
            foreach(Peer peer in _Servers)
            {
                await IpfsApiWrapper.AddPeer(peer);
            }
        }

        #endregion
    }
}
