using System;
using System.IO;

namespace MiP.IO.Win32
{
    public class CopyFileEventArgs : EventArgs
    {
        public CopyFileEventArgs(FileInfo source, FileInfo destination)
        {
            Source = source;
            Destination = destination;
        }

        public void Cancel()
        {
            CallbackAction = CopyFileCallbackAction.Cancel;
        }

        public void Continue()
        {
            CallbackAction = CopyFileCallbackAction.Continue;
        }

        internal CopyFileCallbackAction CallbackAction { get; private set; }

        public FileInfo Source { get; }
        public FileInfo Destination { get; }
        public long TotalFileSize { get; internal set; }
        public long TotalBytesTransferred { get; internal set; }
    }
}