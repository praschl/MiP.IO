using System;

namespace MiP.Tools.IO
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