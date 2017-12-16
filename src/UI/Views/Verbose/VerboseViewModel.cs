using System.Text;
using System.Windows.Input;

namespace Salus
{
    class VerboseViewModel
    {
        private readonly PasswordEntry _Model;
        private readonly VerboseView _View;

        public VerboseViewModel(VerboseView view, PasswordEntry entry)
        {
            _View = view;
            _Model = entry;
        }

        public ICommand OkCommand
        {
            get { return new DelegateCommand(OkCommandImpl); }
        }

        public string EncryptedName
        {
            get { return _Model.EncryptedName; }
        }

        public string NameHash
        {
            get { return _Model.NameHash; }
        }

        public string Name
        {
            get { return _Model.Name; }
        }

        private void OkCommandImpl()
        {
            _View.DialogResult = true;
            _View.Close();
        }
    }
}
