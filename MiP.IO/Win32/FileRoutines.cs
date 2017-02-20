using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace MiP.IO.Win32
{
    // originally found at https://web.archive.org/web/20130304214632/http://msdn.microsoft.com/en-us/magazine/cc163851.aspx
    // refactored for events and async pattern.

    // TODO: Test

    public class FileRoutines
    {
        public event EventHandler<CopyFileEventArgs> CopyFileProgressChanged;

        public async Task CopyFileAsync(FileInfo source, FileInfo destination, CopyFileOptions options, CancellationToken cancellationToken)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if ((options & ~CopyFileOptions.All) != 0) // if any bit in options is set which is not defined by CopyFileOptions
                throw new ArgumentOutOfRangeException(nameof(options));

            new FileIOPermission(FileIOPermissionAccess.Read, source.FullName).Demand();
            new FileIOPermission(FileIOPermissionAccess.Write, destination.FullName).Demand();

            CopyProgressRoutine cpr;
            if (CopyFileProgressChanged != null)
            {
                var fileEventArgs = new CopyFileEventArgs(source, destination);

                cpr = (totalFileSize, totalBytesTransferred,
                        streamSize, streamBytesTransferred, streamNumber, callbackReason, sourceFile, destinationFile, data) =>
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var copyFileEventArgs = fileEventArgs;
                            copyFileEventArgs.TotalFileSize = totalFileSize;
                            copyFileEventArgs.TotalBytesTransferred = totalBytesTransferred;

                            OnCopyFileProgressChanged(copyFileEventArgs);

                            return (int) fileEventArgs.CallbackAction;
                        }
                        catch (OperationCanceledException)
                        {
                            return (int) CopyFileCallbackAction.Cancel;
                        }
                    };
            }
            else
                cpr = null;

            var cancel = false;
            var copyFileExReszkt = await Task.Run(() =>
                    CopyFileEx(source.FullName, destination.FullName, cpr, IntPtr.Zero, ref cancel, (int) options)).ConfigureAwait(false);

            if (!copyFileExReszkt)
                throw new IOException(new Win32Exception().Message);
        }

        private void OnCopyFileProgressChanged(CopyFileEventArgs e)
        {
            CopyFileProgressChanged?.Invoke(this, e);
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CopyFileEx(
            string lpExistingFileName, string lpNewFileName,
            CopyProgressRoutine lpProgressRoutine,
            IntPtr lpData, ref bool pbCancel, int dwCopyFlags);

        private delegate int CopyProgressRoutine(
            long totalFileSize, long totalBytesTransferred, long streamSize,
            long streamBytesTransferred, int streamNumber, int callbackReason,
            IntPtr sourceFile, IntPtr destinationFile, IntPtr data);
    }
}