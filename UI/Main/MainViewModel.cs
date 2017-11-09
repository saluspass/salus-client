using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ipfs_pswmgr
{
    public class MainViewModel
    {
        #region Variables

        private readonly MainModel _Model;

        private int _SelectedPasswordIndex;
        private string _SearchText;

        private readonly ObservableCollection<PasswordEntry> _Passwords;

        #endregion

        #region Ctor

        public MainViewModel()
        {
            _Model = new MainModel();

            _SelectedPasswordIndex = -1;
            _Passwords = new ObservableCollection<PasswordEntry>();

            foreach (string file in Directory.GetFiles(FileSystemConstants.PswmgrDataFolder, "*.json"))
            {
                var val =  PasswordEntry.Load(file);
                _Model.AddEntry(val);
                _Passwords.Add(val);
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<PasswordEntry> Passwords
        {
            get { return _Passwords; }
        }

        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                if (_SearchText != value)
                {
                    _SearchText = value;
                    OnPropertyChanged();

                    Search(value);
                }
            }
        }

        public int SelectedPasswordIndex
        {
            get { return _SelectedPasswordIndex; }
            set
            {
                if (_SelectedPasswordIndex != value)
                {
                    _SelectedPasswordIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void Search(string searchTerm)
        {
            searchTerm = searchTerm?.ToLower();
            _Passwords.Clear();
            foreach (var entry in _Model.Entries)
            {
                if (searchTerm == null)
                {
                    _Passwords.Add(entry);
                }
                else if (entry.Name.ToLower().Contains(searchTerm))
                {
                    _Passwords.Add(entry);
                }
                else if (entry.Username.ToLower().Contains(searchTerm))
                {
                    _Passwords.Add(entry);
                }
                else if (entry.Website.ToLower().Contains(searchTerm))
                {
                    _Passwords.Add(entry);
                }
                else
                {
                    foreach(PasswordEntry.Field field in entry.Fields)
                    {
                        if (field.Name.ToLower().Contains(searchTerm))
                        {
                            _Passwords.Add(entry);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}