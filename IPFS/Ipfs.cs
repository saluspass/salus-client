using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ipfs_pswmgr
{
    public static class Ipfs
    {
        #region Nested

        public delegate string HandleOutput(string data);

        #endregion

        private static IpfsFileListing _FileListing;
        private static Process _DaemonProcess;

        public static string Add(string filename)
        {
            if(!File.Exists(filename))
            {
                return null;
            }

            return ExecuteCommand("add", $"{filename}", delegate(string text)
            {
                var splitText = text.Split(' ');
                if (splitText[0] == "added" && splitText[2].TrimEnd(new[] { '\n' }) == Path.GetFileName(filename))
                {
                    return splitText[1];
                }
                return null;
            });
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
        
        public static string ExecuteCommand(string command, string arguments, HandleOutput handleOutputDelegate)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("ipfs", $"{command} {arguments}");
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            using (StreamReader reader = process.StandardOutput)
            {
                var text = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    return handleOutputDelegate?.Invoke(text);
                }
            }
            using (StreamReader reader = process.StandardError)
            {
                var text = reader.ReadToEnd();
            }
            return null;
        }

        public static string Resolve(string name = null)
        {
            return ExecuteCommand("name", $"resolve {name}", delegate (string text)
            {
                return text.TrimEnd('\n').Substring(6);
            });
        }

        public static async Task<bool> PublishAsync(string hashFilename)
        {
            return await Task.Run(delegate
            {
                return Publish(hashFilename);
            });
        }

        public static bool Publish(string hashFilename)
        {
            string returnText = ExecuteCommand("name", $"publish {hashFilename}", delegate (string text)
            {
                return text;
            });

            return returnText.Contains("Published to");
        }

        public static bool Get(string hash, string filename)
        {
            DateTime lastModified = DateTime.Now;
            if (File.Exists(filename))
            {
                lastModified = File.GetLastWriteTime(filename);
            }

            ExecuteCommand("get", $"-o {filename} {hash}", null);

            return File.Exists(filename) && File.GetLastWriteTime(filename) > lastModified;
        }

        public static void StartDaemon()
        {
            CleanReproLock();

            _DaemonProcess = new Process();
            _DaemonProcess.StartInfo = new ProcessStartInfo("ipfs", "daemon");
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