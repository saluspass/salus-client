using System;

namespace ipfs_pswmgr
{
    internal class DataEventArgs : EventArgs
    {
        public DataEventArgs(string data)
        {
            Data = data;
        }

        public string Data
        {
            get;
        }
    }
}
