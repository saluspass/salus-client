using System.Windows.Input;

namespace ipfs_pswmgr
{
    class OptionsViewModel
    {
        private readonly OptionsView _View;

        #region Ctor

        public OptionsViewModel(OptionsView view)
        {
            _View = view;
        }

        #endregion

        #region Properties

        public Conf Model
        {
            get { return App.Instance.Conf; }
        }

        public ICommand OKCommand
        {
            get { return new DelegateCommand(delegate { Model.Save("pswmgr.conf"); _View.DialogResult = true; _View.Close(); }); }
        }

        public ICommand CancelCommand
        {
            get { return new DelegateCommand(delegate { _View.DialogResult = false; _View.Close(); }); }
        }

        #endregion
    }
}
