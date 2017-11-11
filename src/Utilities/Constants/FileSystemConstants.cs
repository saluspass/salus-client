using System;
using System.IO;

namespace Salus
{
    public static class FileSystemConstants
    {
        public static readonly string UserHomeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static readonly string IpfsConfigFolder = Path.Combine(UserHomeFolder, ".ipfs");
        public static readonly string PswmgrConfigFolder = Path.Combine(UserHomeFolder, ".ipfs-pswmgr");

        public static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string PswmgrDataFolder = Path.Combine(AppDataFolder, "ipfs-pswmgr");

        public static readonly string ConfFilePath = Path.Combine(PswmgrConfigFolder, "conf.json");
    }
}
