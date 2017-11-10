using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace ipfs_pswmgr
{
    internal class PasswordEntryManager
    {
        #region Variables

        private readonly ObservableCollection<PasswordEntry> _Passwords;
        static Lazy<PasswordEntryManager> _Instance = new Lazy<PasswordEntryManager>();

        #endregion

        #region Ctor

        public PasswordEntryManager()
        {
            _Passwords = new ObservableCollection<PasswordEntry>();
        }

        #endregion

        #region Properties

        public static PasswordEntryManager Instance
        {
            get { return _Instance.Value; }
        }

        public ObservableCollection<PasswordEntry> Passwords
        {
            get { return _Passwords; }
        }

        #endregion

        #region Methods

        public async void LoadPasswords(Action afterCompleted)
        {
            _Passwords.Clear();

            Directory.CreateDirectory(FileSystemConstants.PswmgrDataFolder);

            var files = Directory.GetFiles(FileSystemConstants.PswmgrDataFolder, "*.json");
            await Task.Run(delegate
            {
                Parallel.ForEach(files, delegate (string file)
                {
                        var val = PasswordEntry.Load(file);
                        ExceptionUtilities.TryCatchIgnore(() => App.Instance.Dispatcher.Invoke(() => _Passwords.Add(val)));
                });
            });

            afterCompleted?.Invoke();
        }

        public void AddEntry(PasswordEntry entry)
        {
            int index = _Passwords.IndexOf(entry);
            if (index != -1)
            {
                _Passwords[index] = entry;
            }
            else
            {
                _Passwords.Add(entry);
            }
        }

        public void SaveEntries()
        {
            foreach (PasswordEntry entry in _Passwords)
            {
                entry.Save();
            }
        }

        #endregion
    }
}
