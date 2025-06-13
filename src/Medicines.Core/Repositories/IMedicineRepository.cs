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
        Task<IEnumerable<Medicine>> GetPagedMedicinesAsync(string name, string laboratory, int pageNumber, int pageSize);
        Task<int> CountMedicinesAsync(string name, string laboratory);
        Task<Medicine> GetByCode(string code);
    }
}
