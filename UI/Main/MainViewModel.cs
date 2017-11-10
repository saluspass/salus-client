using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ipfs_pswmgr
{
    public class MainViewModel
    {
        #region Variables

        private readonly MainModel _Model;
        private readonly MainWindow _View;

        private int _SelectedPasswordIndex;
        private string _SearchText;

        private readonly ObservableCollection<PasswordEntry> _Passwords;

        #endregion

        #region Ctor

        public MainViewModel(MainWindow view)
        {
            _Model = new MainModel();
            _View = view;

            _SelectedPasswordIndex = -1;
            _Passwords = new ObservableCollection<PasswordEntry>();

            PasswordEntryManager.Instance.LoadPasswords(delegate
            {
                foreach (var entry in PasswordEntryManager.Instance.Passwords)
                {
                    _Passwords.Add(entry);
                }

                SyncIpfsListing();
            });
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

        #region Commands

        public ICommand ShowOptions
        {
            get { return new ShowOptionsCommand(_View, this); }
        }

        public ICommand Exit
        {
            get { return new ExitCommand(); }
        }

        public ICommand AddPassword
        {
            get { return new NewPasswordCommand(_View, this); }
        }

        public ICommand RefreshPasswords
        {
            get
            {
                return new DelegateCommand(() => PasswordEntryManager.Instance.LoadPasswords(null));
            }
        }

        public ICommand Delete
        {
            get { return new DelegateCommand(DeletePassword); }
        }

        public ICommand Modify
        {
            get { return new DelegateCommand(ModifyPassword); }
        }

        public ICommand Copy
        {
            get { return new DelegateCommand(CopyPassword); }
        }

        public ICommand ShowPasswordPermanently
        {
            get { return new DelegateCommand(ShowSelectedPasswordPermanently); }
        }

        public ICommand GeneratePassword
        {
            get { return new DelegateCommand(GeneratePasswordImpl); }
        }

        #endregion

        #endregion

        #region Methods

        private async void SyncIpfsListing()
        {
            await SyncIpfsListingImpl();
        }

        private async Task SyncIpfsListingImpl()
        {
            await Ipfs.GetFileListing();
        }

        private void Search(string searchTerm)
        {
            searchTerm = searchTerm?.ToLower();
            _Passwords.Clear();
            foreach (var entry in PasswordEntryManager.Instance.Passwords)
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

        internal bool AddNewPassword(PasswordEntry newPassword)
        {
            PasswordEntryManager.Instance.AddEntry(newPassword);
            return true;
        }

        private void DeletePassword()
        {
            if (_SelectedPasswordIndex == -1)
            {
                MessageBox.Show(_View, "No selected password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PasswordEntry entry = _Passwords[_SelectedPasswordIndex];
            entry.Delete();
            _Passwords.RemoveAt(_SelectedPasswordIndex);
        }

        private void ModifyPassword()
        {
            if (_SelectedPasswordIndex == -1)
            {
                MessageBox.Show(_View, "No selected password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PasswordEntry entry = _Passwords[_SelectedPasswordIndex];

            NewPasswordView passwordView = new NewPasswordView(entry)
            {
                Owner = _View
            };
            try
            {
                if (passwordView.ShowDialog() == true)
                {
                    entry.Save();
                    MessageBox.Show(_View, "Modified password successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(_View, "Modification Cancelled", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch
            {
                MessageBox.Show(_View, "Problem modifying the password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyPassword()
        {
            if (_SelectedPasswordIndex == -1)
            {
                MessageBox.Show(_View, "No selected password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PasswordEntry entry = _Passwords[_SelectedPasswordIndex];

            System.Windows.Clipboard.SetText(entry.Password);
        }

        private void ShowSelectedPasswordPermanently()
        {
            if (_SelectedPasswordIndex == -1)
            {
                MessageBox.Show(_View, "No selected password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PasswordEntry entry = _Passwords[_SelectedPasswordIndex];
        }

        private void GeneratePasswordImpl()
        {
            const string alphanumericCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890()!@#$%&*:;/";
            string password = GetRandomString(16, alphanumericCharacters);
            MessageBox.Show(_View, "New password generated and copied to the clipboard", "Password Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            System.Windows.Clipboard.SetText(password);
        }

        //Code originally taken from http://stackoverflow.com/questions/54991/generating-random-passwords
        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
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