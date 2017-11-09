using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace ipfs_pswmgr
{
    [JsonObject(MemberSerialization.OptIn)]
    class Conf
    {
        #region Variables

        [JsonProperty]
        private string _Username;

        [JsonProperty]
        private string _ServerCertificate;

        [JsonProperty]
        private string _AuthenticationAddress;

        [JsonProperty]
        private string _AuthenticationPort;

        [JsonProperty]
        private string _PasswordManagerAddress;

        [JsonProperty]
        private string _PasswordManagerPort;

        private string _Password;
        private string _TwoFactorAuthToken;

        #endregion

        #region Ctor

        ~Conf()
        {
            Save("pswmgr.conf");
        }

        #endregion

        #region Properties

        [Browsable(false)]
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        public string ServerCertificate
        {
            get { return _ServerCertificate; }
            set { _ServerCertificate = value; }
        }

        public string AuthenticationAddress
        {
            get { return _AuthenticationAddress; }
            set { _AuthenticationAddress = value; }
        }

        public string AuthenticationPort
        {
            get { return _AuthenticationPort; }
            set { _AuthenticationPort = value; }
        }

        public string TwoFactorAuthToken
        {
            get { return _TwoFactorAuthToken; }
            set { _TwoFactorAuthToken = value; }
        }

        [Browsable(false)]
        public string AuthenticationChannel
        {
            get { return string.Format("{0}:{1}", _AuthenticationAddress, _AuthenticationPort); }
        }

        public string PasswordManagerAddress
        {
            get { return _PasswordManagerAddress; }
            set { _PasswordManagerAddress = value; }
        }

        public string PasswordManagerPort
        {
            get { return _PasswordManagerPort; }
            set { _PasswordManagerPort = value; }
        }

        [Browsable(false)]
        public string PasswordManagerChannel
        {
            get { return string.Format("{0}:{1}", _PasswordManagerAddress, _PasswordManagerPort); }
        }

        [Browsable(false)]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
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

        internal bool IsValid()
        {
            return !string.IsNullOrEmpty(_ServerCertificate) && !string.IsNullOrEmpty(_AuthenticationAddress) &&
                !string.IsNullOrEmpty(_AuthenticationPort) && !string.IsNullOrEmpty(_PasswordManagerAddress) &&
                !string.IsNullOrEmpty(_PasswordManagerPort);
        }

        #endregion
    }
}
