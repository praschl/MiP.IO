using System.IO;

namespace MiP.IO.Win32
{
    public delegate CopyFileCallbackAction CopyFileCallback(
        FileInfo source, FileInfo destination,
        long totalFileSize, long totalBytesTransferred);
}