using System.Collections.Generic;

namespace ipfs_pswmgr
{
    internal class MainModel
    {
        #region Variables

        private readonly List<PasswordEntry> m_Entries;

        #endregion

        public MainModel()
        {
            m_Entries = new List<PasswordEntry>();
        }

        #region Properties

        public IReadOnlyCollection<PasswordEntry> Entries
        {
            get { return m_Entries; }
        }

        #endregion

        #region Methods

        public void AddEntry(PasswordEntry entry)
        {
            int index = m_Entries.IndexOf(entry);
            if(index != -1)
            {
                m_Entries[index] = entry;
            }
            else
            {
                m_Entries.Add(entry);
            }
        }

        public void SaveEntries()
        {
            foreach(PasswordEntry entry in m_Entries)
            {
                entry.Save();
            }
        }

        #endregion
    }
}
