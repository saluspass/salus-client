using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;

namespace Salus
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Variables

        private readonly PasswordEntryManager _Model;
        private readonly MainWindow _View;

        private int _SelectedPasswordIndex;
        private string _SearchText;
        private string _Status;
        private bool _PasswordsLoaded;

        private readonly ObservableCollection<PasswordEntry> _Passwords;

        #endregion

        #region Ctor

        public MainViewModel(MainWindow view)
        {
            _Model = PasswordEntryManager.Instance;
            _View = view;

            _SelectedPasswordIndex = -1;
            _Passwords = new ObservableCollection<PasswordEntry>();

            LoadPasswords();
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

        public string Status
        {
            get { return _Status; }
            private set
            {
                if(value != _Status)
                {
                    _Status = value;
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
                return new DelegateCommand(_Model.LoadPasswords);
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

        public ICommand ExportDataFiles
        {
            get { return new ExportDataFilesCommand(_View); }
        }

        #endregion

        #endregion

        #region Methods

        private async void SyncIpfsListing()
        {
            Status = "Synching with Network...";
            await ApiWrapper.GetFileListingAsync();
            Status = "Up to date";
        }

        private void Search(string searchTerm)
        {
            searchTerm = searchTerm?.ToLower();
            _Passwords.Clear();
            foreach (var entry in _Model.Passwords)
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
            _Model.AddEntry(newPassword);
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
            string password = GetRandomString(16, App.Instance.Conf.RandomPasswordCharacterSet);
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

        private void OnPasswordsFinishedLoading()
        {
            _Model.Passwords.CollectionChanged += Passwords_CollectionChanged;

            Status = null;

            foreach (var entry in _Model.Passwords)
            {
                _Passwords.Add(entry);
            }

            SyncIpfsListing();
        }

        private void OnPasswordsMoved()
        {
        }

        private void OnPasswordsReset()
        {
        }

        private void OnPasswordReplaced()
        {
        }

        private void OnPasswordRemoved(System.Collections.IList oldItems)
        {
            foreach (PasswordEntry entry in oldItems.OfType<PasswordEntry>())
            {
                App.Instance.Dispatcher.Invoke(() => _Passwords.Remove(entry));
            }
        }

        private void OnPasswordAdded(System.Collections.IList newItems)
        {
            foreach(PasswordEntry entry in newItems.OfType<PasswordEntry>())
            {
                App.Instance.Dispatcher.Invoke(() => _Passwords.Add(entry));
            }
        }

        internal void LoadPasswords()
        {
            if (!_PasswordsLoaded && FirstRunViewModel.WasCompleted())
            {
                _PasswordsLoaded = true;

                Status = "Loading Passwords...";
                _Model.FinishedLoading += Instance_FinishedLoading;
                _Model.LoadPasswords();
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

        #region Event Handlers

        private void Instance_FinishedLoading(object sender, EventArgs e)
        {
            OnPasswordsFinishedLoading();
        }

        private void Passwords_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnPasswordAdded(e.NewItems);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnPasswordRemoved(e.OldItems);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    OnPasswordReplaced();
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnPasswordsReset();
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    OnPasswordsMoved();
                    break;
            }
        }

        #endregion
    }
}