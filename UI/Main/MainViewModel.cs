using System.Collections.Generic;
using System.IO;

namespace ipfs_pswmgr
{
    public class MainViewModel
    {
        #region Variables

        private readonly MainModel m_Model;

        #endregion

        #region Ctor

        public MainViewModel()
        {
            m_Model = new MainModel();

            List<PasswordEntry> entries = new List<PasswordEntry>();
            foreach(string file in Directory.GetFiles(@"C:\Users\mfilion\.ipfs-pswmgr", "*.json"))
            {
                var val =  PasswordEntry.Load(file);
                entries.Add(val);
            }

            PasswordEntry entry = new PasswordEntry();
            entry.Name = "TEST";
            entry.Password = "TEST123";
            entry.Username = "test@test.com";
            entry.Save();
        }

        #endregion
    }
}
