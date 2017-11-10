using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipfs_pswmgr
{
    public static class IpfsApi
    {
        #region Nested

        public delegate string HandleOutput(string data);

        #endregion

        private static IpfsFileListing _FileListing;
        private static Process _DaemonProcess;
        private static readonly Ipfs.Api.IpfsClient _Client = new Ipfs.Api.IpfsClient();

        public static async Task<string> Add(string filename)
        {
            if(!File.Exists(filename))
            {
                return null;
            }

            Ipfs.Api.FileSystemNode node = await _Client.FileSystem.AddFileAsync(filename);
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

        public static IpfsFileListing GetFileListing()
        {
            if (_FileListing == null)
            {
                _FileListing = IpfsFileListing.Load().Result;
            }
            return _FileListing;
        }

        public static async Task<string> Resolve(string name = null)
        {
            System.Threading.CancellationToken token = new System.Threading.CancellationToken();
            string json = await _Client.DoCommandAsync("name/resolve", token, name);

            var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
            return jObject["Path"] != null ? jObject.Value<string>("Path").Substring(6) : null;
        }

        public static async Task<bool> PublishAsync(string hashFilename)
        {
            System.Threading.CancellationToken token = new System.Threading.CancellationToken();
            string json = await _Client.UploadAsync("name/publish", token, Encoding.UTF8.GetBytes(hashFilename));
            var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
            return jObject["Name"] != null && jObject["Value"] != null;
        }

        public static async Task<bool> Get(string hash, string filename)
        {
            string content = await _Client.FileSystem.ReadAllTextAsync(hash);
            if (!string.IsNullOrEmpty(content))
            {
                File.WriteAllText(filename, content);
                return true;
            }
            return false;
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
                    var json = File.ReadAllText(filename);
                    Newtonsoft.Json.Linq.JObject obj = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
                    int pid;
                    int.TryParse(obj["OwnerPID"].ToString(), out pid);

                    if (!Process.GetProcesses().Any(o => o.Id == pid))
                    {
                        File.Delete(filename);
                    }
                });
            }
        }
    }
}