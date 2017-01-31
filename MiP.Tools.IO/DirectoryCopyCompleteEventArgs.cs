using System;

namespace MiP.Tools.IO
{
    public class DirectoryCopyCompleteEventArgs : EventArgs
    {
        public DirectoryCopyCompleteEventArgs(bool cancelled)
        {
            Cancelled = cancelled;
        }

        public bool Cancelled { get; }
    }
}