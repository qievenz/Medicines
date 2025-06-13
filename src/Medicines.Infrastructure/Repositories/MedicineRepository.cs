using Microsoft.EntityFrameworkCore;
using Medicines.Core.Entities;
using Medicines.Core.Repositories;
using Medicines.Infrastructure.Persistence;

namespace Medicines.Infrastructure.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Medicine data)
        {
            _context.Medicines.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var Medicines = await _context.Medicines.FindAsync(id);
            if (Medicines != null)
            {
                _context.Medicines.Remove(Medicines);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Medicine>> GetAll()
        {
            return await _context.Medicines.ToListAsync();
        }

        public async Task<Medicine?> GetById(Guid id)
        {
            return await _context.Medicines.FindAsync(id);
        }

        public async Task Update(Medicine data)
        {
            _context.Medicines.Update(data);
            await _context.SaveChangesAsync();
        }
    }
}
