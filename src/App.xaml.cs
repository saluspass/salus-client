using System.Windows;

namespace Salus
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

        public Conf Conf
        {
            get { return _Conf; }
        }

        #endregion

        #region Methods

        private void OnApplicationStartup()
        {
            IpfsApiWrapper.StartDaemon();
            SetupBootstrapServers();

            RestServer a = new RestServer();
            a.Start();

            AutoUpdater.Launch();

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

        private async void SetupBootstrapServers()
        {
            BootstrapServers instance = await BootstrapServers.Instance();
            await instance.Install();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            IpfsApiWrapper.StopDaemon();

            base.OnExit(e);
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
