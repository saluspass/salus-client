using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Salus
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IpfsFileListing : ISaveableObject
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

        #region Properties

        public bool Dirty
        {
            get;
            set;
        }

        #endregion

        #region Methods

        private static async Task<IpfsFileListing> FetchListingFile(string listingFileHash, bool keep = false)
        {
            IpfsFileListing returnValue = null;
            try
            {
                string localListingFilename = keep ? ListingFilename : Path.GetTempFileName();
                if (!string.IsNullOrEmpty(listingFileHash) && await IpfsApiWrapper.Get(listingFileHash, localListingFilename))
                {
                    using (StreamReader reader = new StreamReader(File.OpenRead(localListingFilename)))
                    {
                        string json = reader.ReadToEnd();
                        returnValue = JsonConvert.DeserializeObject<IpfsFileListing>(json);
                    }
                }
            }
            catch
            {
                //In case the peer's file hasn't yet been published to ipns
            }
            returnValue.Dirty = false;
            return returnValue;
        }

        public static async Task<IpfsFileListing> Load()
        {
            IpfsFileListing returnValue = new IpfsFileListing();
            try
            {
                if(File.Exists(ListingFilename))
                {
                    using (StreamReader reader = new StreamReader(File.OpenRead(ListingFilename)))
                    {
                        string json = reader.ReadToEnd();
                        returnValue = JsonConvert.DeserializeObject<IpfsFileListing>(json);
                    }
                }

                string localPeerId = await IpfsApiWrapper.GetPeerId();

                PeerListing peerListing = await IpfsApiWrapper.GetPeerListingAsync();
                foreach (string peer in peerListing.Peers)
                {
                    IpfsFileListing peerFileListing = null;
                    if (peer != localPeerId)
                    {
                        string listingFileHash = await IpfsApiWrapper.ResolveAsync(peer);
                        peerFileListing = await FetchListingFile(listingFileHash, false);
                    }
                    else if(File.Exists(ListingFilename))
                    {
                        string listingFileHash = await IpfsApiWrapper.ResolveAsync();
                        peerFileListing = await FetchListingFile(listingFileHash, true);
                    }

                    returnValue.MergeFile(peerFileListing);
                }
            }
            catch
            {
            }

            await returnValue.Sync();

            return returnValue;
        }

        private async Task AddFileNotAlreadyExisting(string filename, object lockObject)
        {
            string hashFilename = await IpfsApiWrapper.AddAsync(filename);

            IpfsFile file = new IpfsFile
            {
                LocalFilename = filename,
                RemoteFilename = hashFilename
            };
            file.ComputerAndStoreHash(filename);
            lock (lockObject)
            {
                Dirty = true;
                _Files.Add(file);
            }
        }

        private void ResolveConflict(string localFilename, string filename)
        {
            IpfsFile file = _Files.First(o => o.LocalFilename == localFilename);
            if (file.ComputeHash(filename) != file.Hash)
            {
                Dirty = true;
                //TODO: Complete
            }
        }

        private void ResolveConflict(IpfsFile remoteFile)
        {
            IpfsFile file = _Files.First(o => o.LocalFilename == remoteFile.LocalFilename);
            if (file.Hash != remoteFile.Hash)
            {
                Dirty = true;
                //TODO: Complete
            }
        }

        private async Task ForEachAsync<T>(IEnumerable<T> source, Func<T, Task> body)
        {
            foreach(T item in source)
            {
                await body(item);
            }
        }

        private async Task ProcessLocalFiles(string[] files, object lockObject)
        {
            await ForEachAsync(files, async delegate (string filename)
            {
                string localFilename = Path.GetFileNameWithoutExtension(filename);
                if (!_Files.Any(o => o.LocalFilename == localFilename))
                {
                    await AddFileNotAlreadyExisting(filename, lockObject);
                }
                else
                {
                    ResolveConflict(localFilename, filename);
                }
            });
        }

        private async Task ProcessRemoteFileList()
        {
            await ForEachAsync(_Files, async delegate (IpfsFile file)
            {
                string filename = Path.Combine(FileSystemConstants.PswmgrDataFolder, Path.ChangeExtension(file.LocalFilename, ".json"));
                if (File.Exists(filename))
                {
                    ResolveConflict(file.LocalFilename, filename);
                }
                else
                {
                    await IpfsApiWrapper.Get(file.RemoteFilename, filename);
                    PasswordEntryManager.Instance.AddEntry(PasswordEntry.Load(filename));
                }
            });
        }

        private async Task Sync()
        {
            object lockObject = new object();
            var files = Directory.GetFiles(FileSystemConstants.PswmgrDataFolder);

            await ProcessLocalFiles(files, lockObject);

            await ProcessRemoteFileList();

            await Save();
        }

        public async Task Save()
        {
            if (Dirty)
            {
                using (StreamWriter writer = new StreamWriter(ListingFilename))
                {
                    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                }

                string hashFilename = await IpfsApiWrapper.AddAsync(ListingFilename);
                await IpfsApiWrapper.PublishAsync(hashFilename);
                Dirty = false;
            }
        }

        private void MergeFile(IpfsFileListing other)
        {
            foreach(IpfsFile file in other._Files)
            {
                string mfLocalFilename = file.LocalFilename;
                if (!_Files.Any(o => o.LocalFilename == mfLocalFilename))
                {
                    Dirty = true;
                    _Files.Add(file);
                }
                else
                {
                    ResolveConflict(file);
                }
            }
        }

        #endregion
    }
}
