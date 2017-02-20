using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MiP.IO
{
    public class FileCopy
    {
        private readonly int _bufferSize;
        
        // TODO: overwrite flag

        public FileCopy() : this(0x1000)
        {
        }

        public FileCopy(int bufferSize)
        {
            if (bufferSize<0x1000)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer sizes lower than 0x1000 should not be used for performance reasons.");

            _bufferSize = bufferSize;
        }

        public event EventHandler<FileCopyProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<FileCopyCompleteEventArgs> Complete;

        public async Task CopyAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken)
        {
            await CopyAsync(new FileInfo(sourceFile), new FileInfo(destinationFile), cancellationToken);
        }

        public async Task CopyAsync(FileInfo sourceFile, FileInfo destinationFile, CancellationToken cancellationToken)
        {
            var buffer = new byte[_bufferSize];

            try
            {
                using (var sourceStream = sourceFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var fileLength = sourceStream.Length;
                    using (var destinationStream = destinationFile.Open(FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                    {
                        long bytesWritten = 0;

                        int blockSize;
                        while ((blockSize = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await destinationStream.WriteAsync(buffer, 0, blockSize, cancellationToken);

                            cancellationToken.ThrowIfCancellationRequested();

                            bytesWritten += blockSize;
                            var percent = bytesWritten * 100.0 / fileLength;

                            var eventargs = new FileCopyProgressChangedEventArgs(percent);
                            ProgressChanged?.Invoke(this, eventargs);
                        }
                    }
                }
                Complete?.Invoke(this, new FileCopyCompleteEventArgs(false));
            }
            catch (TaskCanceledException)
            {
                DeleteFile(destinationFile);
                Complete?.Invoke(this, new FileCopyCompleteEventArgs(true));
            }
        }

        private static void DeleteFile(FileSystemInfo file)
        {
            file.Delete();
        }
    }
}