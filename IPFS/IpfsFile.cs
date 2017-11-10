using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;

namespace ipfs_pswmgr
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IpfsFile
    {
        #region Properties

        [JsonProperty]
        public string LocalFilename
        {
            get;
            set;
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
            get
            {
                byte[] allBytes = File.ReadAllBytes(LocalFilename);
                return Base32.ToBase32String(SHA256.Create().ComputeHash(allBytes));
            }
        }

        #endregion
    }
}
