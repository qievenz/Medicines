using Medicines.Core.Entities;

namespace Medicines.Core.Services
{
    public interface IDataService
    {
        Task<IEnumerable<Medicine>> GetAllDatasAsync();
        Task<Medicine?> GetDataByIdAsync(int id);
        Task<Medicine> CreateDataAsync(Medicine data);
        Task<bool> UpdateDataAsync(int id, Medicine data);
        Task<bool> DeleteDataAsync(int id);
    }
}
