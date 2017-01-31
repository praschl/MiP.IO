using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MiP.Tools.IO
{
    public class FileCopy
    {
        private readonly int _bufferSize;

        public FileCopy(int bufferSize)
        {
            if (bufferSize<0x1000)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer sizes lower than 0x1000 hurt performance.");

            _bufferSize = bufferSize;
        }

        public event EventHandler<FileCopyProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<FileCopyCompleteEventArgs> Complete;

        public async Task CopyAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken)
        {
            var buffer = new byte[_bufferSize];

            try
            {
                using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                {
                    var fileLength = sourceStream.Length;
                    using (var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write))
                    {
                        long bytesWritten = 0;

                        int blockSize;
                        while ((blockSize = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await destinationStream.WriteAsync(buffer, 0, blockSize, cancellationToken);

                            cancellationToken.ThrowIfCancellationRequested();

                            bytesWritten += blockSize;
                            var percent = bytesWritten*100.0/fileLength;

                            var eventargs = new FileCopyProgressChangedEventArgs(percent);
                            ProgressChanged?.Invoke(this, eventargs);
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                    DeleteFile(destinationFile);
            }
            finally
            {
                Complete?.Invoke(this, new FileCopyCompleteEventArgs(cancellationToken.IsCancellationRequested));
            }
        }

        private static void DeleteFile(string filename)
        {
            Console.WriteLine("Deleting file: "+filename);

            var file = new FileInfo(filename);
            file.Delete();
        }
    }
}