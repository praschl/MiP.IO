using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace MiP.Tools.IO.Win32
{
    // found at https://web.archive.org/web/20130304214632/http://msdn.microsoft.com/en-us/magazine/cc163851.aspx

    // TODO: event instead of delegate callback, this means big refactoring
    // TODO: thread safety of CopyFileEx ?
    // TODO: make async
    // TODO: cancellation token
    public sealed class FileRoutines
    {
        public static void CopyFile(FileInfo source, FileInfo destination, 
            CopyFileOptions options, CopyFileCallback callback)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) 
                throw new ArgumentNullException(nameof(destination));
            if ((options & ~CopyFileOptions.All) != 0) 
                throw new ArgumentOutOfRangeException(nameof(options));

            new FileIOPermission(
                FileIOPermissionAccess.Read, source.FullName).Demand();
            new FileIOPermission(
                FileIOPermissionAccess.Write, destination.FullName).Demand();

            CopyProgressRoutine cpr = callback == null ? 
                null : new CopyProgressRoutine(new CopyProgressData(
                    source, destination, callback).CallbackHandler);

            bool cancel = false;
            if (!CopyFileEx(source.FullName, destination.FullName, cpr, 
                IntPtr.Zero, ref cancel, (int)options))
            {
                throw new IOException(new Win32Exception().Message);
            }
        }

        private class CopyProgressData
        {
            private readonly FileInfo _source;
            private readonly FileInfo _destination;
            private readonly CopyFileCallback _callback;

            public CopyProgressData(FileInfo source, FileInfo destination, 
                CopyFileCallback callback)
            {
                _source = source; 
                _destination = destination;
                _callback = callback;
            }

            public int CallbackHandler(
                long totalFileSize, long totalBytesTransferred, 
                long streamSize, long streamBytesTransferred, 
                int streamNumber, int callbackReason,
                IntPtr sourceFile, IntPtr destinationFile, IntPtr data)
            {
                return (int)_callback(_source, _destination, 
                    totalFileSize, totalBytesTransferred);
            }
        }

        private delegate int CopyProgressRoutine(
            long totalFileSize, long totalBytesTransferred, long streamSize, 
            long streamBytesTransferred, int streamNumber, int callbackReason,
            IntPtr sourceFile, IntPtr destinationFile, IntPtr data);
        
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool CopyFileEx(
            string lpExistingFileName, string lpNewFileName,
            CopyProgressRoutine lpProgressRoutine,
            IntPtr lpData, ref bool pbCancel, int dwCopyFlags);
    }
}