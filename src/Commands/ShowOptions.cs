using System.Windows;

namespace Salus
{
    internal class ShowOptionsCommand : AbstractCommand
    {
        #region Variables

        private readonly Window _ParentView;
        private readonly MainViewModel _ViewModel;

        #endregion

        #region Ctor

        public ShowOptionsCommand(Window parentView, MainViewModel viewModel)
        {
            _ParentView = parentView;
            _ViewModel = viewModel;
        }

        #endregion

        #region Method

        protected override void Execute()
        {
            OptionsView view = new OptionsView()
            {
                Owner = _ParentView
            };
            view.ShowDialog();
        }

        #endregion
    }
}
