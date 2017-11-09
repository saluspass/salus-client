using Newtonsoft.Json;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ipfs_pswmgr
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PasswordEntry
    {
        #region Variables

        SecureString m_Name;
        SecureString m_Password;
        SecureString m_Username;

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

        #endregion

        #region Methods

        public void Save()
        {
            string filename = Path.ChangeExtension(Base32.ToBase32String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Name))), ".json");
            using (StreamWriter writer = new StreamWriter(Path.Combine(FirstRunViewModel.PswmgrFolder, filename)))
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

        #endregion
    }
}
