using Medicines.Core.Entities;

namespace Medicines.Core.Repositories
{
    public interface IIngestionProcessRepository
    {
        Task<IngestionProcess> GetById(Guid id);
        Task Add(IngestionProcess process);
        Task Update(IngestionProcess process);
    }
}
