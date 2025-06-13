namespace Medicines.Core.Services
{
    public interface IChannelDataService<T>
    {
        void CompletePublishing();
        Task PublishMessageAsync(T message);
        Task StartProcessingAsync(CancellationToken cancellationToken = default);
    }
}
