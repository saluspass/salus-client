using Newtonsoft.Json;
using System.IO;

namespace ipfs_pswmgr
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Conf
    {
        #region Ctor

        public Conf()
        {
            CheckForUpdates = true;
            RandomPasswordCharacterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890()!@#$%&*:;/";
        }

        ~Conf()
        {
            Save(FileSystemConstants.ConfFilePath);
        }

        #endregion

        #region Properties
    
        [JsonProperty]
        public bool CheckForUpdates
        {
            get;
            set;
        }

        [JsonProperty]
        public string RandomPasswordCharacterSet
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public static Conf Load(string filename)
        {
            if (!File.Exists(filename))
                return new Conf();

            string fileData = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Conf>(fileData);
        }

        public void Save(string filename)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        #endregion
    }
}
