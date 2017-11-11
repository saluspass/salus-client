using Ipfs.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salus
{
    public static class IpfsApiWrapper
    {
        #region Nested

        public delegate string HandleOutput(string data);

        #endregion

        #region Variables

        private static IpfsFileListing _FileListing;
        private static Process _DaemonProcess;
        private static readonly Lazy<IpfsClient> _Client = new Lazy<IpfsClient>();

        #endregion

        #region Properties

        private static IpfsClient Client
        {
            get
            {
                //Add any initialization configuration needed here
                return _Client.Value;
            }
        }

        #endregion

        #region Methods

        public static async Task<string> AddAsync(string filename)
        {
            if(!File.Exists(filename))
            {
                return null;
            }

            FileSystemNode node = await Client.FileSystem.AddFileAsync(filename);
            return node.Hash;
        }

        public static async Task<IpfsFileListing> GetFileListingAsync()
        {
            if (_FileListing == null)
            {
                _FileListing = await IpfsFileListing.Load();
            }
            return _FileListing;
        }

        public static async Task<string> ResolveAsync(string name = null)
        {
            System.Threading.CancellationToken token = new System.Threading.CancellationToken();
            string json = await Client.DoCommandAsync("name/resolve", token, name);

            JObject jObject = JObject.Parse(json);
            return jObject["Path"] != null ? jObject.Value<string>("Path").Substring(6) : null;
        }

        public static async Task<bool> PublishAsync(string hashFilename)
        {
            System.Threading.CancellationToken token = new System.Threading.CancellationToken();
            string json = await Client.UploadAsync("name/publish", token, Encoding.UTF8.GetBytes(hashFilename));
            JObject jObject = JObject.Parse(json);
            return jObject["Name"] != null && jObject["Value"] != null;
        }

        public static async Task<bool> Get(string hash, string filename)
        {
            string content = await Client.FileSystem.ReadAllTextAsync(hash);
            if (!string.IsNullOrEmpty(content))
            {
                File.WriteAllText(filename, content);
                return true;
            }
            return false;
        }

        public static async Task<string> GetPeerId()
        {
            System.Threading.CancellationToken token = new System.Threading.CancellationToken();
            string json = await Client.DoCommandAsync("id", token);
            JObject obj = JsonConvert.DeserializeObject<JObject>(json);

            return obj.Value<string>("ID");
        }

        public static void StartDaemon()
        {
            CleanReproLock();

            _DaemonProcess = new Process();
            _DaemonProcess.StartInfo = new ProcessStartInfo("ipfs", "daemon --enable-pubsub-experiment");
            _DaemonProcess.StartInfo.CreateNoWindow = false;
            _DaemonProcess.Start();
        }

        public static void StopDaemon()
        {
            if(_DaemonProcess?.HasExited == false)
            {
                _DaemonProcess.CloseMainWindow();
            }

            CleanReproLock();
        }

        private static void CleanReproLock()
        {
            string filename = Path.Combine(FileSystemConstants.IpfsConfigFolder, "repo.lock");
            if(File.Exists(filename))
            {
                ExceptionUtilities.TryCatchIgnore(delegate
                {
                    string json = File.ReadAllText(filename);
                    JObject obj = JsonConvert.DeserializeObject<JObject>(json);
                    int pid;
                    int.TryParse(obj["OwnerPID"].ToString(), out pid);

                    if (!Process.GetProcesses().Any(o => o.Id == pid))
                    {
                        File.Delete(filename);
                    }
                });
            }
        }

        #endregion
    }
}