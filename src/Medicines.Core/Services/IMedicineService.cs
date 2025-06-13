using Medicines.Core.Entities;

namespace Medicines.Core.Services
{
    public interface IMedicineService
    {
        Task<IEnumerable<Medicine>> GetAllDatasAsync();
        Task<Medicine?> GetDataByIdAsync(Guid id);
        Task<Medicine> CreateDataAsync(Medicine data);
        Task<bool> UpdateDataAsync(Guid id, Medicine data);
        Task<bool> DeleteDataAsync(Guid id);
    }
}
