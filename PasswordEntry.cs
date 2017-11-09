using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ipfs_pswmgr
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PasswordEntry
    {
        #region Nested

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class Field
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
                    StoreProperty(out m_Name, value);
                }
            }

            public string Name
            {
                get { return m_Name.ConvertToUnsecureString(); }
                set { m_Name = value.ConvertToSecureString(); }
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
                    StoreProperty(out m_Value, value);
                }
            }

            public string Value
            {
                get { return m_Value.ConvertToUnsecureString(); }
                set { m_Value = value.ConvertToSecureString(); }
            }
        }

        #endregion

        #region Variables

        SecureString m_Name;
        SecureString m_Password;
        SecureString m_Username;
        SecureString m_Website;
        List<Field> m_Fields;

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
                StoreProperty(out m_Name, value);
            }
        }

        public string Name
        {
            get { return m_Name.ConvertToUnsecureString(); }
            set { m_Name = value.ConvertToSecureString(); }
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
                StoreProperty(out m_Password, value);
            }
        }

        public string Password
        {
            get { return m_Password.ConvertToUnsecureString(); }
            set { m_Password = value.ConvertToSecureString(); }
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
                StoreProperty(out m_Username, value);
            }
        }

        public string Username
        {
            get { return m_Username.ConvertToUnsecureString(); }
            set { m_Username = value.ConvertToSecureString(); }
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
                StoreProperty(out m_Website, value);
            }
        }

        public string Website
        {
            get { return m_Website.ConvertToUnsecureString(); }
            set { m_Website = value.ConvertToSecureString(); }
        }


        [JsonProperty]
        public List<Field> Fields
        {
            get { return m_Fields; }
            set { m_Fields = value; }
        }
        #endregion

        #region Methods

        public void Save()
        {
            Directory.CreateDirectory(FileSystemConstants.PswmgrDataFolder);

            string filename = Path.ChangeExtension(Base32.ToBase32String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Name))), ".json");
            using (StreamWriter writer = new StreamWriter(Path.Combine(FileSystemConstants.PswmgrDataFolder, filename)))
            {
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
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
            return entry;
        }

        private static string GetProperty(SecureString ss)
        {
            using (EncryptionKey key = EncryptionKey.Load())
            {
                return key.EncryptString(ss.ConvertToUnsecureString());
            }
        }

        private static void StoreProperty(out SecureString ss, string encryptedString)
        {
            using (EncryptionKey key = EncryptionKey.Load())
            {
                var decryptedString = key.DecryptString(encryptedString);
                ss = decryptedString.ConvertToSecureString();
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

        #endregion
    }
}