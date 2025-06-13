using Microsoft.EntityFrameworkCore;
using Medicines.Core.Entities;
using Medicines.Core.Repositories;
using Medicines.Infrastructure.Persistence;

namespace Medicines.Infrastructure.Repositories
{
    public class MedicineRepository : Core.Repositories.MedicineRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Medicine data)
        {
            _context.Datas.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var datas = await _context.Datas.FindAsync(id);
            if (datas != null)
            {
                _context.Datas.Remove(datas);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Medicine>> GetAll()
        {
            return await _context.Datas.ToListAsync();
        }

        public async Task<Medicine?> GetById(int id)
        {
            return await _context.Datas.FindAsync(id);
        }

        public async Task Update(Medicine data)
        {
            _context.Datas.Update(data);
            await _context.SaveChangesAsync();
        }
    }
}
