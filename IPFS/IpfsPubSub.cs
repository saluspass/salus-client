using System;
using System.Diagnostics;

namespace ipfs_pswmgr
{
    public class IpfsPubSub
    {
        #region Variables

        private readonly Process _Process;
        private readonly Action<string> _OutputReceived;
        private readonly Action<string> _ErrorReceived;

        #endregion

        #region Ctor

        public IpfsPubSub(string subCategory, Action<string> outputReceived = null, Action<string> errorReceived = null)
        {
            SubCategory = subCategory;
            _OutputReceived = outputReceived;
            _ErrorReceived = errorReceived;

            _Process = new Process();
            _Process.StartInfo = new ProcessStartInfo();
            _Process.StartInfo = new ProcessStartInfo("ipfs", $"pubsub sub {SubCategory}");
            _Process.StartInfo.RedirectStandardOutput = true;
            //_Process.StartInfo.RedirectStandardError = true;
            _Process.StartInfo.UseShellExecute = false;
            //_Process.StartInfo.CreateNoWindow = false;

            _Process.OutputDataReceived += Process_OutputDataReceived;
            _Process.ErrorDataReceived += Process_ErrorDataReceived;
        }

        #endregion

        #region Properties

        public string SubCategory
        {
            get;
        }

        #endregion

        #region Methods

        public void Start()
        {
            _Process.Start();
        }

        public void Stop()
        {
            if(_Process.HasExited)
            {
                return;
            }

            _Process.CloseMainWindow();
        }

        private void OnProcessDataReceived(string data)
        {
            _OutputReceived?.Invoke(data);
            OnOutputReceived(data);
        }

        private void OnProcessErrorReceived(string data)
        {
            _ErrorReceived?.Invoke(data);
            OnErrorReceived(data);
        }

        #endregion

        #region Event Handlers

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnProcessDataReceived(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnProcessErrorReceived(e.Data);
        }

        #endregion

        #region Events

        internal event EventHandler<DataEventArgs> OutputReceived;
        internal event EventHandler<DataEventArgs> ErrorReceived;

        protected void OnOutputReceived(string data)
        {
            OutputReceived?.Invoke(this, new DataEventArgs(data));
        }

        protected void OnErrorReceived(string data)
        {
            ErrorReceived?.Invoke(this, new DataEventArgs(data));
        }

        #endregion
    }
}
