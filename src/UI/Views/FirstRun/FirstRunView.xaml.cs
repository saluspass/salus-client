using System.Windows;

namespace Salus
{
    /// <summary>
    /// Interaction logic for FirstRunView.xaml
    /// </summary>
    public partial class FirstRunView : Window
    {
        #region Variables

        private readonly FirstRunViewModel _ViewModel;

        #endregion

        #region Ctor

        public FirstRunView()
        {
            _ViewModel = new FirstRunViewModel();
            DataContext = _ViewModel;

            InitializeComponent();

            _ViewModel.AccountConfigued += _ViewModel_AccountConfigued;
        }

        #endregion

        #region Methods

        private void OnAccountConfigured()
        {
            MessageBox.Show("Account is now configured. If you lose your private key, your data will be unrecoverable.", "Account Configured", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            DialogResult = true;
            Close();
        }

        #endregion

        #region Event Handlers

        private void _ViewModel_AccountConfigued(object sender, System.EventArgs e)
        {
            OnAccountConfigured();
        }

        #endregion
    }
}
