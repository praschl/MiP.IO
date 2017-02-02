using System.IO;

namespace MiP.Tools.IO.Win32
{
    public delegate CopyFileCallbackAction CopyFileCallback(
        FileInfo source, FileInfo destination,
        long totalFileSize, long totalBytesTransferred);
}