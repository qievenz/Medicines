using Medicines.Core.Entities;

namespace Medicines.Core.Repositories
{
    public interface IDataRepository
    {
        Task<IEnumerable<Data>> GetAll();
        Task<Data?> GetById(int id);
        Task Add(Data data);
        Task Delete(int id);
        Task Update(Data data);
    }
}
