using Newtonsoft.Json;
using Semver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;

namespace ipfs_pswmgr
{
    internal class AutoUpdater
    {
        internal static async void Launch()
        {
            if (!App.Instance.Conf.CheckForUpdates)
                return;

            List<GitHub.Release> releases = null;
            using (WebClient webClient = new CustomizedWebClient())
            {
                webClient.Headers.Add("User-Agent: Other");
                var json = await webClient.DownloadStringTaskAsync(ReleasesConstants.ApiEndpoint);
                releases = JsonConvert.DeserializeObject<List<GitHub.Release>>(json);
            }

            Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            SemVersion semAssemblyVersion = SemVersion.Parse($"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.MinorRevision}");

            GitHub.Release bestRelease = null;
            SemVersion bestReleaseVersion = semAssemblyVersion;

            foreach(GitHub.Release release in releases)
            {
                SemVersion releaseSemVersion = SemVersion.Parse(release.TagName);
                if(releaseSemVersion > bestReleaseVersion)
                {
                    bestReleaseVersion = releaseSemVersion;
                    bestRelease = release;
                }
            }

            CleanupOldFiles();

            if (bestRelease != null && bestReleaseVersion > semAssemblyVersion)
            {
                if(MessageBox.Show("There is a new version available, do you wish to update?", "New Version", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    UpdateView view = new UpdateView(bestRelease);
                    view.ExecuteUpdate();
                }
            }
        }

        private static void CleanupOldFiles()
        {
            string parentFolderName = Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location).FullName;
            DeleteFilesRecursively(parentFolderName);
        }

        private static void DeleteFilesRecursively(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteFilesRecursively(directory);
                if(!Directory.GetFiles(directory).Any())
                {
                    Directory.Delete(directory);
                }
            }

            foreach (string file in Directory.GetFiles(path, "*.old"))
            {
                File.Delete(file);
            }
        }
    }
}
