using System.Windows;

namespace Salus
{
    internal class NewPasswordCommand : AbstractCommand
    {
        #region Variables

        private readonly Window _View;
        private readonly MainViewModel _MainViewModel;

        #endregion

        #region Ctor

        public NewPasswordCommand(Window parentView, MainViewModel mainViewModel)
        {
            _View = parentView;
            _MainViewModel = mainViewModel;
        }

        #endregion

        #region Methods

        protected override void Execute()
        {
            NewPasswordView passwordView = new NewPasswordView()
            {
                Owner = _View
            };
            if (passwordView.ShowDialog() == true)
            {
                AddPassword(passwordView.ViewModel.Model);
            }
        }

        private void AddPassword(PasswordEntry entry)
        {
            var result = _MainViewModel.AddNewPassword(entry);
            if (result)
            {
                entry.Save();
                MessageBox.Show(_View, "Added password successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(_View, "Problem adding password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
