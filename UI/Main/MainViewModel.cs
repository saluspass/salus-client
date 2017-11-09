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

            foreach(string file in Directory.GetFiles(FileSystemConstants.PswmgrDataFolder, "*.json"))
            {
                var val =  PasswordEntry.Load(file);
                m_Model.AddEntry(val);
            }

            PasswordEntry entry = new PasswordEntry();
            entry.Name = "TEST";
            entry.Password = "TEST123";
            entry.Username = "test@test.com";
            entry.Fields.Add(new PasswordEntry.Field("FieldName", "FieldValue"));

            m_Model.AddEntry(entry);
            m_Model.SaveEntries();
        }

        #endregion
    }
}
