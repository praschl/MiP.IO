using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using F = System.IO.File;

namespace MiP.IO.Test
{
    [TestClass]
    public class FileCopyTest
    {
        [TestMethod]
        public async Task Copies_file_to_destination()
        {
            var source = CreateRandomFile(0x10000);

            var copy = new FileCopy(0x1000);
            copy.ProgressChanged += (o, e) =>
            {
                Console.WriteLine(e.Percent);
            };
            
            var destination = Guid.NewGuid().ToString();

            await copy.CopyAsync(source, destination, CancellationToken.None);

            var sourceBytes = F.ReadAllBytes(source);
            var destBytes = F.ReadAllBytes(destination);

            destBytes.ShouldBeEquivalentTo(sourceBytes, o => o.WithStrictOrdering());

            F.Delete(destination);
            F.Delete(source);
        }

        [TestMethod]
        public async Task File_is_deleted_after_cancel()
        {
            const int timetoWaitBetweenBlocks = 100;
            const int timetoWaitBeforeCancelling = 250;

            var source = CreateRandomFile(0x10000);

            var copy = new FileCopy(0x1000);
            copy.ProgressChanged += (o,e)=>
            {
                Console.WriteLine(e.Percent);
                // this makes the copy process wait a bit, so there is time for cancellation
                // do not change to await Task.Delay - this would not cause a wait for the operations.
                Thread.Sleep(timetoWaitBetweenBlocks);
            };

            var destination = Guid.NewGuid().ToString();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timetoWaitBeforeCancelling);

            await copy.CopyAsync(source, destination, cancellationTokenSource.Token);
            
            F.Exists(destination).Should().BeFalse();
            F.Delete(source);
        }

        private string CreateRandomFile(int size)
        {
            var filename = Guid.NewGuid().ToString();

            var r = new Random();

            using (var stream = F.Create(filename))
            {
                var bytes = new byte[size];
                r.NextBytes(bytes);

                stream.Write(bytes, 0, size);
                stream.Close();
            }

            return filename;
        }
    }
}