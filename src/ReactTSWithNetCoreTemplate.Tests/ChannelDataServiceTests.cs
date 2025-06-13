using ReactTSWithNetCoreTemplate.Application.Services;
using System.Diagnostics;

namespace ReactTSWithNetCoreTemplate.Tests
{
    [TestFixture]
    public class ChannelDataServiceTests
    {
        [Test]
        public async Task TestHighVolumeProcessing()
        {
            int totalMessages = 100_000;
            int workerCount = 100;

            int processedCount = 0;

            Func<int, Task> processMessageAsync = async (int message) =>
            {
                Console.WriteLine(message);
                Interlocked.Increment(ref processedCount);
            };

            var processor = new ChannelDataService<int>(workerCount, processMessageAsync);

            Stopwatch stopwatch = Stopwatch.StartNew();
            Task processingTask = processor.StartProcessingAsync();

            Task publishingTask = Task.Run(async () =>
            {
                for (int i = 0; i < totalMessages; i++)
                {
                    await processor.PublishMessageAsync(i);
                }

                processor.CompletePublishing();
            });

            // Espera a que ambas tareas finalicen (publicación y consumo).
            await Task.WhenAll(processingTask, publishingTask);

            stopwatch.Stop();

            Console.WriteLine($"Processed {processedCount} out of {totalMessages} messages in {stopwatch.Elapsed}");

        }


    }
}
