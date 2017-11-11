using System.Windows;

namespace Salus
{
    /// <summary>
    /// Interaction logic for NewPasswordView.xaml
    /// </summary>
    public partial class NewPasswordView : Window
    {
        #region Variables

        private readonly NewPasswordViewModel _ViewModel;

        #endregion

        #region Ctor

        public NewPasswordView(PasswordEntry entry = null)
        {
            _ViewModel = new NewPasswordViewModel(this, entry);
            DataContext = _ViewModel;

            InitializeComponent();

            _PasswordBox.Password = _ViewModel.Model.Password;
        }

        #endregion

        #region Properties

        internal NewPasswordViewModel ViewModel
        {
            get { return _ViewModel; }
        }

        #endregion

        #region Methods

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            _ViewModel.OnPasswordBoxPasswordChanged();
        }

        private void LoginView_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _ViewModel.OnPreviewKeyUp(e.Key);
        }

        #endregion
    }
}
