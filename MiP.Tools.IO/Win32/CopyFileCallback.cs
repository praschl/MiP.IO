using System;
using System.IO;

namespace MiP.Tools.IO.Win32
{
    public delegate CopyFileCallbackAction CopyFileCallback(
        FileInfo source, FileInfo destination,
        long totalFileSize, long totalBytesTransferred);

    public class CopyFileEventArgs : EventArgs
    {
        public CopyFileEventArgs(FileInfo source, FileInfo destination, long totalFileSize, long totalBytesTransferred)
        {
            Source = source;
            Destination = destination;
            TotalFileSize = totalFileSize;
            TotalBytesTransferred = totalBytesTransferred;
        }

        public FileInfo Source { get; }
        public FileInfo Destination { get; }
        public long TotalFileSize { get; }
        public long TotalBytesTransferred { get; }
    }
}