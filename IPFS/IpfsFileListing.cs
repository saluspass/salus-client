using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

        public static IpfsFileListing Load()
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
                    returnValue.Save();
                }
            }
            return returnValue;
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(ListingFilename))
            {
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }

            string hashFilename = Ipfs.Add(ListingFilename);
            Ipfs.Publish(hashFilename);
        }

        #endregion
    }
}
