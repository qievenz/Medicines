using Medicines.Core.Entities;

namespace Medicines.Core.Repositories
{
    public interface MedicineRepository
    {
        Task<IEnumerable<Medicine>> GetAll();
        Task<Medicine?> GetById(int id);
        Task Add(Medicine data);
        Task Delete(int id);
        Task Update(Medicine data);
    }
}
