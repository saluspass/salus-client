using System.IO;
using System.Windows;

namespace ipfs_pswmgr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Variables

        private static App _Instance;
        private Conf _Conf;

        #endregion

        #region Ctor

        public App()
        {
            _Instance = this;

            _Conf = Conf.Load(FileSystemConstants.ConfFilePath);
        }

        #endregion

        #region Properties

        public static App Instance
        {
            get { return _Instance; }
        }

        internal Conf Conf
        {
            get { return _Conf; }
        }

        #endregion

        #region Methods

        private void OnApplicationStartup()
        {
            MainWindow mainWindow = new MainWindow();

            if (!FirstRunViewModel.WasCompleted())
            {
                FirstRunView dialog = new FirstRunView();
                if (dialog.ShowDialog() != true)
                {
                    mainWindow = null;
                    Shutdown();
                }
            }

            mainWindow?.Show();
        }

        #endregion

        #region Event Handlers

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            OnApplicationStartup();
        }

        #endregion
    }
}
