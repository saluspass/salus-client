using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ipfs_pswmgr
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PasswordEntry : INotifyPropertyChanged, IDirtableObject
    {
        #region Nested

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class Field : IDirtableObject
        {
            private SecureString m_Name;
            private SecureString m_Value;

            public Field()
            {
                m_Name = new SecureString();
                m_Value = new SecureString();
            }

            public Field(string name, string value)
            {
                m_Name = name.ConvertToSecureString();
                m_Value = value.ConvertToSecureString();
            }

            [JsonProperty(PropertyName = "Name")]
            public string EncryptedName
            {
                get
                {
                    return GetProperty(m_Name);
                }
                set
                {
                    StoreProperty(this, ref m_Name, value);
                }
            }

            public string Name
            {
                get { return m_Name.ConvertToUnsecureString(); }
                set { m_Name.AssignIfDifferent(value, this); }
            }

            [JsonProperty(PropertyName = "Value")]
            public string EncryptedValue
            {
                get
                {
                    return GetProperty(m_Value);
                }
                set
                {
                    StoreProperty(this, ref m_Value, value);
                }
            }

            public string Value
            {
                get { return m_Value.ConvertToUnsecureString(); }
                set { m_Value.AssignIfDifferent(value, this); }
            }

            public bool Dirty
            {
                get;
                set;
            }
        }

        #endregion

        #region Variables

        private SecureString m_Name;
        private SecureString m_Password;
        private SecureString m_Username;
        private SecureString m_Website;
        private List<Field> m_Fields;
        private bool _SearchedForImage;
        private bool _Dirty;

        #endregion

        #region Ctor

        public PasswordEntry()
        {
            m_Name = new SecureString();
            m_Password = new SecureString();
            m_Username = new SecureString();
            m_Website = new SecureString();
            m_Fields = new List<Field>();
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "Name")]
        public string EncryptedName
        {
            get
            {
                return GetProperty(m_Name);
            }
            set
            {
                StoreProperty(this, ref m_Name, value);
            }
        }

        public string Name
        {
            get { return m_Name.ConvertToUnsecureString(); }
            set { m_Name.AssignIfDifferent(value, this); }
        }

        [JsonProperty(PropertyName = "Password")]
        public string EncryptedPassword
        {
            get
            {
                return GetProperty(m_Password);
            }
            set
            {
                StoreProperty(this, ref m_Password, value);
            }
        }

        public string Password
        {
            get { return m_Password.ConvertToUnsecureString(); }
            set { m_Password.AssignIfDifferent(value, this); }
        }

        [JsonProperty(PropertyName = "Username")]
        public string EncryptedUsername
        {
            get
            {
                return GetProperty(m_Username);
            }
            set
            {
                StoreProperty(this, ref m_Username, value);
            }
        }

        public string Username
        {
            get { return m_Username.ConvertToUnsecureString(); }
            set { m_Username.AssignIfDifferent(value, this); }
        }

        [JsonProperty(PropertyName = "Website")]
        public string EncryptedWebsite
        {
            get
            {
                return GetProperty(m_Website);
            }
            set
            {
                StoreProperty(this, ref m_Website, value);
            }
        }

        public string Website
        {
            get { return m_Website.ConvertToUnsecureString(); }
            set { m_Website.AssignIfDifferent(value, this); }
        }


        [JsonProperty]
        public List<Field> Fields
        {
            get { return m_Fields; }
            set { m_Fields = value; }
        }

        public BitmapImage Icon
        {
            get { return GetIcon(); }
        }

        public bool Dirty
        {
            get { return _Dirty || Fields.Any(o => o.Dirty); }
            set
            {
                _Dirty = value;
                Fields.ForEach(o => o.Dirty = value);
            }
        }

        #endregion

        #region Methods

        public void Save()
        {
            Directory.CreateDirectory(FileSystemConstants.PswmgrDataFolder);

            if (Dirty)
            {
                string filename = Path.ChangeExtension(Base32.ToBase32String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Name))), ".json");
                using (StreamWriter writer = new StreamWriter(Path.Combine(FileSystemConstants.PswmgrDataFolder, filename)))
                {
                    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                }

                Dirty = false;
            }
        }

        public static PasswordEntry Load(string filename)
        {
            PasswordEntry entry = null;
            using (StreamReader reader = new StreamReader(File.OpenRead(filename)))
            {
                string json = reader.ReadToEnd();
                entry = JsonConvert.DeserializeObject<PasswordEntry>(json);
            }
            entry.Dirty = false;
            return entry;
        }

        internal void Delete()
        {
            string filename = Path.ChangeExtension(Base32.ToBase32String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Name))), ".json");
            string fullFilepath = Path.Combine(FileSystemConstants.PswmgrDataFolder, filename);

            if (File.Exists(fullFilepath))
            {
                File.Delete(fullFilepath);
            }
        }

        private static string GetProperty(SecureString ss)
        {
            using (EncryptionKey key = EncryptionKey.Load())
            {
                return key.EncryptString(ss.ConvertToUnsecureString());
            }
        }

        private static void StoreProperty(IDirtableObject parent, ref SecureString ss, string encryptedString)
        {
            using (EncryptionKey key = EncryptionKey.Load())
            {
                var decryptedString = key.DecryptString(encryptedString);
                var tempSecureString = decryptedString.ConvertToSecureString();
                if(tempSecureString.ConvertToUnsecureString() != ss.ConvertToUnsecureString())
                {
                    ss = tempSecureString;
                    parent.Dirty = true;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if(obj is PasswordEntry)
            {
                PasswordEntry other = (PasswordEntry)obj;
                return Name == other.Name;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        private BitmapImage GetIcon()
        {
            if (!_SearchedForImage)
            {
                string file = Path.Combine(Path.GetTempPath(), "soteriapass", Website.Replace(':', '_').Replace('.', '_').Replace('/', '_'), "favicon.ico");
                string fileAlt = Path.ChangeExtension(file, ".sec.ico");
                Directory.CreateDirectory(Directory.GetParent(file).FullName);
                if (File.Exists(file))
                {
                    BitmapImage returnValue = ExceptionUtilities.TryAssignCatchIgnore(delegate { return new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute)); }, null);
                    if (returnValue != null)
                        return returnValue;
                }
                if (File.Exists(fileAlt))
                {

                }
                if (!_SearchedForImage)
                {
                    _SearchedForImage = true;
                    Task.Run(delegate
                    {
                        DownloadFile(Website, file);
                        if (ExceptionUtilities.TryAssignCatchIgnore(delegate { return new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute)); }, null) != null)
                        {
                            OnPropertyChanged(nameof(Icon));
                            return;
                        }

                        DownloadFileAlternative(Website, fileAlt);
                        if (ExceptionUtilities.TryAssignCatchIgnore(delegate { return new BitmapImage(new Uri(fileAlt, UriKind.RelativeOrAbsolute)); }, null) != null)
                        {
                            OnPropertyChanged(nameof(Icon));
                            return;
                        }
                    });
                }
            }
            return new BitmapImage(new Uri("/ipfs-pswmgr;component/Resources/PasswordManagerIcon.ico", UriKind.RelativeOrAbsolute));
        }

        private void DownloadFile(string value, string file)
        {
            using (WebClient client = new WebClient())
            {
                ExceptionUtilities.TryCatchIgnore(() => client.DownloadFile(Path.Combine(value, "favicon.ico").Replace('\\', '/'), file));
            }
        }

        private void DownloadFileAlternative(string value, string file)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = ExceptionUtilities.TryAssignCatchIgnore(delegate { return web.Load(value); }, null);

            if (doc == null)
                return;

            foreach (var node in doc.DocumentNode.Descendants("link"))
            {
                var relValue = node.GetAttributeValue("rel", null);
                if (relValue?.ToLower() == "shortcut icon")
                {
                    using (WebClient client = new WebClient())
                    {
                        ExceptionUtilities.TryCatchIgnore(() => client.DownloadFile(node.GetAttributeValue("href", null), file));
                    }
                    continue;
                }

                var typeValue = node.GetAttributeValue("image/x-icon", null);
                if (typeValue != null)
                {
                    using (WebClient client = new WebClient())
                    {
                        ExceptionUtilities.TryCatchIgnore(() => client.DownloadFile(node.GetAttributeValue("type", null), file));
                    }
                }
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}