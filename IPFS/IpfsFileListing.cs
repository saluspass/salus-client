﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                string listingFileHash = await IpfsApi.Resolve();
                if(string.IsNullOrEmpty(listingFileHash))
                {
                    listingFileHash = await IpfsApi.Resolve();
                }

                if (!string.IsNullOrEmpty(listingFileHash) && await IpfsApi.Get(listingFileHash, ListingFilename))
                {
                    using (StreamReader reader = new StreamReader(File.OpenRead(ListingFilename)))
                    {
                        string json = reader.ReadToEnd();
                        returnValue = JsonConvert.DeserializeObject<IpfsFileListing>(json);
                    }
                }

            }
            catch
            {
                if(returnValue == null)
                {
                    returnValue = new IpfsFileListing();
                }
            }

            await returnValue.Sync();

            return returnValue;
        }

        private async Task AddFileNotAlreadyExisting(string filename, object lockObject)
        {
            string hashFilename = await IpfsApi.Add(filename);

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

        private void ResolveConflict(string localFilename, string filename)
        {
            IpfsFile file = _Files.First(o => o.LocalFilename == localFilename);
            if (file.ComputeHash(filename) != file.Hash)
            {
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

        private async Task Sync()
        {
            object lockObject = new object();
            var files = Directory.GetFiles(FileSystemConstants.PswmgrDataFolder);

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

            await ForEachAsync(_Files, async delegate (IpfsFile file)
            {
                string filename = Path.Combine(FileSystemConstants.PswmgrDataFolder, Path.ChangeExtension(file.LocalFilename, ".json"));
                if (File.Exists(filename))
                {
                    if (file.ComputeHash(filename) != file.Hash)
                    {
                        //Download or upload
                    }
                }
                else
                {
                    await IpfsApi.Get(file.RemoteFilename, filename);
                    PasswordEntryManager.Instance.AddEntry(PasswordEntry.Load(filename));
                }
            });

            await Save();
        }

        public async Task Save()
        {
            using (StreamWriter writer = new StreamWriter(ListingFilename))
            {
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }

            string hashFilename = await IpfsApi.Add(ListingFilename);
            await IpfsApi.PublishAsync(hashFilename);
        }

        #endregion
    }
}
