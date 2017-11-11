using System;

namespace Salus
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
