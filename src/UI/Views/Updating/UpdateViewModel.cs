using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace Salus
{
    internal class UpdateViewModel : INotifyPropertyChanged
    {
        #region Variables

        private string _StatusText;
        private readonly GitHub.Release _Release;

        #endregion

        #region Ctor

        public UpdateViewModel(GitHub.Release release)
        {
            _Release = release;
        }

        #endregion

        #region Properties

        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                if(_StatusText != value)
                {
                    _StatusText = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        public async void ExecuteUpdate()
        {
            string platformName = string.Empty;
            if(System.Environment.Is64BitOperatingSystem)
            {
                platformName = "win64";
            }
            else
            {
                platformName = "win32";
            }

            string assetToDownload = string.Empty;
            foreach(GitHub.Asset asset in _Release.Assets)
            {
                if(asset.BrowserDownloadUrl.Contains(platformName))
                {
                    assetToDownload = asset.BrowserDownloadUrl;
                    break;
                }
            }

            if(!string.IsNullOrEmpty(assetToDownload))
            {
                string downloadPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(assetToDownload));
                using (WebClient webClient = new CustomizedWebClient())
                {
                    webClient.Headers.Add("User-Agent: Other");
                    StatusText = "Downloading Update...";
                    await webClient.DownloadFileTaskAsync(assetToDownload, downloadPath);
                }

                StatusText = "Extracting Files...";

                string parentFolderName = Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location).FullName;
                RenameFilesRecursively(parentFolderName);

                System.IO.Compression.ZipFile.ExtractToDirectory(downloadPath, parentFolderName);

                StatusText = "Restarting Application...";

                var filename = Directory.GetFiles(parentFolderName, "*.exe").First();
                System.Diagnostics.Process.Start(Path.Combine(parentFolderName, filename), "/cleanupUpdateFiles");
                App.Current.Shutdown();
            }
        }

        private void RenameFilesRecursively(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                RenameFilesRecursively(directory);
            }

            foreach (string file in Directory.GetFiles(path))
            {
                File.Move(file, Path.ChangeExtension(file, $"{Path.GetExtension(file)}.old"));
            }
        }

        private void DeleteFilesRecursively(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteFilesRecursively(directory);
                Directory.Delete(directory);
            }

            foreach (string file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}