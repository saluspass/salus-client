using System;
using System.Windows.Input;

namespace Salus
{
    internal abstract class AbstractCommand : ICommand
    {
        #region Methods

        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        protected virtual bool CanExecute()
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Execute();
        }

        protected abstract void Execute();

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}