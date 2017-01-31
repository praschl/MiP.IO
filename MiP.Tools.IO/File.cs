using System.Threading;
using System.Threading.Tasks;

namespace MiP.Tools.IO
{
    public static class File
    {
        public static void Copy(string sourceFile, string destinationFile, CancellationToken cancellationToken)
        {
            CopyAsync(sourceFile, destinationFile, cancellationToken)
                .Wait(cancellationToken);
        }

        public static async Task CopyAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken)
        {
            await new FileCopy(1024*1024).CopyAsync(sourceFile, destinationFile, cancellationToken);
        }
    }
}