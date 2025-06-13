using Medicines.Core.Entities;

namespace Medicines.Core.Repositories
{
    public interface IMedicineRepository
    {
        Task<IEnumerable<Medicine>> GetAll();
        Task<Medicine?> GetById(Guid id);
        Task Add(Medicine data);
        Task Delete(Guid id);
        Task Update(Medicine data);
    }
}
