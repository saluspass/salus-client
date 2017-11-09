using System.Windows;

namespace ipfs_pswmgr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Variables

        private static readonly App _Instance;

        #endregion

        #region Ctor

        public App()
        {
            _Instance = this;
        }

        #endregion

        #region Methods

        private OnApplicationStartup()
        {

        }

        #endregion

        #region Event Handlers

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            if (!FirstRunViewModel.WasCompleted())
            {
                FirstRunView dialog = new FirstRunView();
                if(dialog.ShowDialog() != true)
                {
                    mainWindow = null;
                    Shutdown();
                }
            }

            mainWindow?.Show();
        }

        #endregion
    }
}
