using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Salus
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PeerListing : ISaveableObject
    {
        #region Constants

        private static readonly string ListingFilename = Path.Combine(FileSystemConstants.PswmgrConfigFolder, "peers.json");

        #endregion

        #region Variables

        private List<string> _Peers;
        private string _FilenameHash;

        #endregion

        #region Ctor

        private PeerListing()
        {
            _Peers = new List<string>();
        }

        #endregion

        #region Properties

        [JsonProperty]
        public List<string> Peers
        {
            get { return _Peers; }
            set
            {
                if (_Peers != value)
                {
                    _Peers = value;
                    Dirty = true;
                }
            }
        }

        public bool Dirty
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public static async Task<PeerListing> Load()
        {
            PeerListing returnValue = new PeerListing();
            try
            {
                string filenameHashIpnsEntry = await IpfsApiWrapper.GetAdditionalInformation("salus");
                string listingFileHash = await IpfsApiWrapper.ResolveAsync(filenameHashIpnsEntry);

                if (!string.IsNullOrEmpty(listingFileHash) && await IpfsApiWrapper.Get(listingFileHash, Path.Combine(FileSystemConstants.PswmgrConfigFolder, "peers.json")))
                {
                    using (StreamReader reader = new StreamReader(File.OpenRead(ListingFilename)))
                    {
                        string json = reader.ReadToEnd();
                        returnValue = JsonConvert.DeserializeObject<PeerListing>(json);
                    }
                }

                returnValue.Dirty = false;
            }
            catch
            {
            }

            await returnValue.Sync();

            return returnValue;
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
                string filenameHashIpnsEntry = await IpfsApiWrapper.GetAdditionalInformation("salus");
                await IpfsApiWrapper.PublishAsync(hashFilename, filenameHashIpnsEntry);
                Dirty = false;
            }
        }

        private async Task Sync()
        {
            _FilenameHash = await IpfsApiWrapper.GetAdditionalInformation("salus");

            string peerId = await IpfsApiWrapper.GetPeerId();
            if (!_Peers.Contains(peerId))
            {
                _Peers.Add(peerId);
                Dirty = true;
            }

            await Save();
        }

        #endregion
    }
}
