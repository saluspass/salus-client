using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;

namespace ipfs_pswmgr
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IpfsFile
    {
        #region Variables

        private string _LocalFilename;
        
        #endregion

        #region Properties

        [JsonProperty]
        public string LocalFilename
        {
            get { return _LocalFilename; }
            set { _LocalFilename = Path.GetFileNameWithoutExtension(value); }
        }

        [JsonProperty]
        public string RemoteFilename
        {
            get;
            set;
        }

        [JsonProperty]
        public string Hash
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public string ComputeHash(string filename)
        {
            byte[] allBytes = File.ReadAllBytes(filename);
            return Base32.ToBase32String(SHA256.Create().ComputeHash(allBytes));
        }

        public void ComputerAndStoreHash(string filename)
        {
            Hash = ComputeHash(filename);
        }

        #endregion
    }
}
