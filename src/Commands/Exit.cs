namespace Salus
{
    internal class ExitCommand : AbstractCommand
    {
        #region Methods

        protected override void Execute()
        {
            App.Current.Shutdown();
        }

        #endregion
    }
}
