using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiP.Tools.IO
{
    public class DirectoryCopy
    {
        private readonly Func<FileCopy> _fileCopyProvider;
        // TODO: test
        // TODO: flag recursive
        // TODO: flag no files
        // TODO: report progress

        public EventHandler<DirectoryCopyCompleteEventArgs> Complete;

        public DirectoryCopy() : this(() => new FileCopy())
        {
        }

        public DirectoryCopy(Func<FileCopy> fileCopyProvider)
        {
            _fileCopyProvider = fileCopyProvider;
        }

        public async Task CopyAsync(string sourceDirectory, string destinationDirectory, CancellationToken cancellationToken)
        {
            await CopyAsync(new DirectoryInfo(sourceDirectory), new DirectoryInfo(destinationDirectory), cancellationToken);
        }

        public async Task CopyAsync(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!destinationDirectory.Exists)
                    destinationDirectory.Create();

                var tasks = GetCopyDirectoryTasks(sourceDirectory, destinationDirectory, cancellationToken)
                    .Concat(GetCopyFilesTasks(sourceDirectory, destinationDirectory, cancellationToken))
                    .ToArray();

                await Task.WhenAll(tasks.ToArray());

                Complete?.Invoke(this, new DirectoryCopyCompleteEventArgs(false));
            }
            catch (TaskCanceledException)
            {
                Complete?.Invoke(this, new DirectoryCopyCompleteEventArgs(true));
            }
        }

        private IEnumerable<Task> GetCopyFilesTasks(DirectoryInfo sourceDirectory, FileSystemInfo destinationDirectory, CancellationToken cancellationToken)
        {
            var fileCopy = _fileCopyProvider();

            var sourceFiles = sourceDirectory.EnumerateFiles();

            foreach (var sourceFile in sourceFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var destinationFile = new FileInfo(Path.Combine(destinationDirectory.FullName, sourceFile.Name));

                yield return fileCopy.CopyAsync(sourceFile, destinationFile, cancellationToken);
            }
        }

        private IEnumerable<Task> GetCopyDirectoryTasks(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory, CancellationToken cancellationToken)
        {
            var sourceDirs = sourceDirectory.EnumerateDirectories();
            foreach (var source in sourceDirs)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var destination = destinationDirectory.CreateSubdirectory(source.Name);

                yield return CopyAsync(source, destination, cancellationToken);
            }
        }
    }
}