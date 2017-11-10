using System.Windows;

namespace ipfs_pswmgr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        private readonly MainViewModel _ViewModel;

        #endregion

        #region Ctor

        public MainWindow()
        {
            _ViewModel = new MainViewModel(this);
            DataContext = _ViewModel;

            InitializeComponent();
        }

        #endregion

        #region Methods

        private void OnWindowVisibleChanged()
        {
            _ViewModel.LoadPasswords();
        }

        #endregion

        #region Event Handlers

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnWindowVisibleChanged();
        }

        #endregion
    }
}
