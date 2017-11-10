using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Input;

namespace ipfs_pswmgr
{
    internal class FirstRunViewModel
    {
        #region Properties

        public ICommand NewAccount
        {
            get { return new DelegateCommand(() => OnNewAccount()); }
        }

        public ICommand ImportExisting
        {
            get { return new DelegateCommand(() => OnImportExisting()); }
        }

        #endregion

        #region Methods

        internal static bool WasCompleted()
        {
            if (!Directory.Exists(FileSystemConstants.IpfsConfigFolder))
                return false;

            if (!Directory.Exists(FileSystemConstants.PswmgrConfigFolder))
                return false;

            EncryptionKey key = EncryptionKey.Load();
            if(key == null)
            {
                return false;
            }

            key.Dispose();
            key = null;

            return true;
        }

        private void OnNewAccount()
        {
            using (EncryptionKey key = EncryptionKey.Generate())
            {
                key.Export();
            }
            OnAccountConfigued();
        }

        private void OnImportExisting()
        {

        }

        #endregion

        #region Events

        public event EventHandler AccountConfigued;

        protected void OnAccountConfigued()
        {
            AccountConfigued?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
