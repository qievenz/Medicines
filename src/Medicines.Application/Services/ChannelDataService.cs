using Medicines.Core.Services;
using System.Threading.Channels;

namespace Medicines.Application.Services
{
    public class ChannelDataService<T> : IChannelDataService<T>
    {
        private readonly Channel<T> _channel;
        private readonly int _workerCount;
        private readonly Func<T, Task> _processMessageAsync;

        /// <param name="workerCount">The number of tasks (workers) that will process messages.</param>
        /// <param name="processMessageAsync">An asynchronous function to process each message.</param>
        public ChannelDataService(int workerCount, Func<T, Task> processMessageAsync)
        {
            if (workerCount <= 0)
                throw new ArgumentException("Worker count must be greater than 0.", nameof(workerCount));

            _workerCount = workerCount;
            _processMessageAsync = processMessageAsync ?? throw new ArgumentNullException(nameof(processMessageAsync));

            _channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });
        }

        /// <summary>
        /// Publishes a message to the channel asynchronously.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        public async Task PublishMessageAsync(T message)
        {
            await _channel.Writer.WriteAsync(message);
        }

        /// <summary>
        /// Signals that no more messages will be published, allowing the channel to complete its reading.
        /// </summary>
        public void CompletePublishing()
        {
            _channel.Writer.Complete();
        }

        /// <summary>
        /// Starts processing messages by launching the configured number of worker tasks.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the processing.</param>
        public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            Task[] tasks = new Task[_workerCount];
            for (int i = 0; i < _workerCount; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
                    {
                        try
                        {
                            await _processMessageAsync(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing message: {ex.Message}");
                        }
                    }
                }, cancellationToken);
            }
            await Task.WhenAll(tasks);
        }
    }
}
