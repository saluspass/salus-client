using System.Windows;

namespace ipfs_pswmgr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
    }
}
