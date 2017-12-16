namespace Salus
{
    /// <summary>
    /// Interaction logic for VerboseView.xaml
    /// </summary>
    public partial class VerboseView
    {
        private readonly VerboseViewModel _ViewModel;

        public VerboseView(PasswordEntry entry)
        {
            _ViewModel = new VerboseViewModel(this, entry);
            DataContext = _ViewModel;

            InitializeComponent();
        }
    }
}
