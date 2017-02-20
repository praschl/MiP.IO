using System;

namespace MiP.IO
{
    public class FileCopyCompleteEventArgs : EventArgs
    {
        public FileCopyCompleteEventArgs(bool cancelled)
        {
            Cancelled = cancelled;
        }

        public bool Cancelled { get; }
    }
}