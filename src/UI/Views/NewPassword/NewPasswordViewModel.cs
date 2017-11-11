using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Salus
{
    class NewPasswordViewModel : INotifyPropertyChanged
    {
        #region Variables

        private readonly NewPasswordView _View;
        private readonly PasswordEntry _Model;
        private readonly DelegateCommand _OKCommand;
        private readonly bool _ExistingEntry;

        #endregion

        #region Ctor

        public NewPasswordViewModel(NewPasswordView view, PasswordEntry entry = null)
        {
            _ExistingEntry = entry != null;
            _View = view;
            _Model = entry ?? new PasswordEntry();
            _OKCommand = new DelegateCommand(OnOk, DataCompleted);
        }

        #endregion

        #region Properties

        public PasswordEntry Model
        {
            get { return _Model; }
        }

        public bool DataCompleted(object obj)
        {
            return !string.IsNullOrEmpty(_Model.Name) && !string.IsNullOrEmpty(_Model.Website) && !string.IsNullOrEmpty(_Model.Password) && !string.IsNullOrEmpty(_Model.Username);
        }

        public ICommand OKCommand
        {
            get { return _OKCommand; }
        }

        internal void OnPasswordBoxPasswordChanged()
        {
            _Model.Password = _View._PasswordBox.Password;
            _OKCommand.RaiseCanExecuteChanged();
        }

        public string Name
        {
            get { return _Model.Name; }
            set
            {
                if (_Model.Name != value)
                {
                    _Model.Name = value;
                    OnPropertyChanged();
                    _OKCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string Username
        {
            get { return _Model.Username; }
            set
            {
                if (_Model.Username != value)
                {
                    _Model.Username = value;
                    OnPropertyChanged();
                    _OKCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string Website
        {
            get { return _Model.Website; }
            set
            {
                if (_Model.Website != value)
                {
                    _Model.Website = value;
                    OnPropertyChanged();
                    _OKCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string WindowTitle
        {
            get { return _ExistingEntry ? "Modify Password" : "Add Password"; }
        }

        public string ButtonContent
        {
            get { return _ExistingEntry ? "Modify" : "Add"; }
        }

        public bool AccountNameEnabled
        {
            get { return !_ExistingEntry; }
        }

        #endregion

        #region Methods

        private void OnOk()
        {
            _View.DialogResult = true;
            _View.Close();
        }

        internal void OnPreviewKeyUp(Key key)
        {
            if ((key == Key.Enter || key == Key.Return) && _OKCommand.CanExecute(null))
            {
                _OKCommand.Execute(null);
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
