using System.Windows;

namespace ipfs_pswmgr
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : Window
    {
        #region Variable

        private readonly OptionsViewModel _ViewModel;

        #endregion

        #region Ctor

        public OptionsView()
        {
            _ViewModel = new OptionsViewModel(this);
            DataContext = _ViewModel;

            InitializeComponent();
        }

        #endregion
    }
}
