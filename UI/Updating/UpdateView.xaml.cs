using System.Windows;

namespace ipfs_pswmgr
{
    /// <summary>
    /// Interaction logic for UpdatingView.xaml
    /// </summary>
    public partial class UpdateView : Window
    {
        #region Variables

        private readonly UpdateViewModel _ViewModel;

        #endregion

        #region Ctor

        public UpdateView(GitHub.Release release)
        {
            _ViewModel = new UpdateViewModel(release);
            DataContext = _ViewModel;

            InitializeComponent();
        }

        #endregion

        #region Methods

        public void ExecuteUpdate()
        {
            IsVisibleChanged += UpdateView_IsVisibleChanged;
            ShowDialog();
        }

        private void OnVisibleChanged()
        {
            _ViewModel.ExecuteUpdate();
        }

        #endregion

        #region Event Handlers

        private void UpdateView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnVisibleChanged();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion
    }
}