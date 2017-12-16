using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace Salus
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Conf
    {
        #region Ctor

        bool _VerboseInfo;

        public Conf()
        {
            CheckForUpdates = true;
            RandomPasswordCharacterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890()!@#$%&*:;/";
            VerboseInformation = false;
        }

        ~Conf()
        {
            Save(FileSystemConstants.ConfFilePath);
        }

        #endregion

        #region Properties
    
        [JsonProperty]
        [DisplayName("Check for Updates")]
        [Description("Whether to check for updates from GitHub.")]
        public bool CheckForUpdates
        {
            get;
            set;
        }

        [JsonProperty]
        [DisplayName("Random Password Character Set")]
        [Description("The character set to use when generating the random passwords.")]
        public string RandomPasswordCharacterSet
        {
            get;
            set;
        }

        [JsonProperty]
        [DisplayName("Display Verbose Information")]
        [Description("(REQUIRES CLIENT RESTART). Whether or not the options for inspecting verbose information is available.")]
        public bool VerboseInformation
        {
            get { return _VerboseInfo; }
            set { _VerboseInfo = value; }
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
