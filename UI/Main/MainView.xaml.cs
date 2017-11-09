using System.Windows;

namespace ipfs_pswmgr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        private readonly MainViewModel m_ViewModel;

        #endregion

        #region Ctor

        public MainWindow()
        {
            m_ViewModel = new MainViewModel();
            DataContext = m_ViewModel;

            InitializeComponent();
        }

        #endregion
    }
}
