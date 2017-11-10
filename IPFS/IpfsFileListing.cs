using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ipfs_pswmgr
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IpfsFileListing
    {
        #region Constants

        private static readonly string ListingFilename = Path.Combine(FileSystemConstants.PswmgrConfigFolder, "listing.json");

        #endregion

        #region Variables

        [JsonProperty(PropertyName = "Files")]
        private List<IpfsFile> _Files;

        #endregion

        #region Ctor

        private IpfsFileListing()
        {
            _Files = new List<IpfsFile>();
        }

        #endregion

        #region Methods

        public static async Task<IpfsFileListing> Load()
        {
            IpfsFileListing returnValue = null;
            try
            {
                string listingFileHash = Ipfs.Resolve();
                if(string.IsNullOrEmpty(listingFileHash))
                {
                    listingFileHash = Ipfs.Resolve();
                }

                if (!string.IsNullOrEmpty(listingFileHash) && Ipfs.Get(listingFileHash, ListingFilename))
                {
                    using (StreamReader reader = new StreamReader(File.OpenRead(ListingFilename)))
                    {
                        string json = reader.ReadToEnd();
                        returnValue = JsonConvert.DeserializeObject<IpfsFileListing>(json);
                    }
                }

            }
            catch (System.Exception ex)
            {
                if(returnValue == null)
                {
                    returnValue = new IpfsFileListing();
                    await returnValue.Save();
                }
            }

            await returnValue.Sync();

            return returnValue;
        }

        private async Task Sync()
        {
            object lockObject = new object();
            var files = Directory.GetFiles(FileSystemConstants.PswmgrDataFolder);
            await Task.Run(() =>
                Parallel.ForEach(files, delegate (string filename)
                {
                    string localFilename = Path.GetFileNameWithoutExtension(filename);
                    if (!_Files.Any(o => o.LocalFilename == localFilename))
                    {
                        string hashFilename = Ipfs.Add(filename);

                        IpfsFile file = new IpfsFile
                        {
                            LocalFilename = filename,
                            RemoteFilename = hashFilename
                        };
                        file.ComputerAndStoreHash(filename);
                        lock (lockObject)
                        {
                            _Files.Add(file);
                        }
                    }
                    else
                    {

                    }
                }));

            await Save();
        }

        public async Task Save()
        {
            using (StreamWriter writer = new StreamWriter(ListingFilename))
            {
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }

            string hashFilename = Ipfs.Add(ListingFilename);
            await Ipfs.Publish(hashFilename);
        }

        #endregion
    }
}
