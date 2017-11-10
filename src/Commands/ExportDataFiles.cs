using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace ipfs_pswmgr
{
    internal class ExportDataFilesCommand : AbstractCommand
    {
        #region Variables

        private readonly Window _Parent;

        #endregion

        #region Ctor

        public ExportDataFilesCommand(Window parentWindow)
        {
            _Parent = parentWindow;
        }

        #endregion

        #region Methods

        protected override void Execute()
        {
            string archiveFilename = Path.ChangeExtension(Path.GetTempFileName(), ".dat.bak");
            string folderName = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            while(Directory.Exists(folderName))
            {
                folderName = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            };
            Directory.CreateDirectory(folderName);

            File.Copy(Path.Combine(FileSystemConstants.IpfsConfigFolder, "config"), Path.Combine(folderName, "config"));
            File.Copy(Path.Combine(FileSystemConstants.PswmgrConfigFolder, "public.pem"), Path.Combine(folderName, "public.pem"));
            File.Copy(Path.Combine(FileSystemConstants.PswmgrConfigFolder, "private.pem"), Path.Combine(folderName, "private.pem"));
            File.Copy(Path.Combine(FileSystemConstants.PswmgrConfigFolder, "conf.json"), Path.Combine(folderName, "conf.json"));

            ZipFile.CreateFromDirectory(folderName, archiveFilename);
            MessageBox.Show($"Data exported to {archiveFilename}. \r\n\r\nPlease keep this backup safe.\r\n\r\n(An explorer window will open after you close this dialog).", "Data Exported", MessageBoxButton.OK, MessageBoxImage.Information);
            Process.Start("explorer.exe", $"/select,{archiveFilename}");

            Directory.Delete(folderName, true);
        }

        #endregion
    }
}
