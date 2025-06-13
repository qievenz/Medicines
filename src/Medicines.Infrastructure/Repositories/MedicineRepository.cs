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
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                medicine.IsDeleted = true;
                _context.Medicines.Update(medicine);
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

        public async Task<IEnumerable<Medicine>> GetPagedMedicinesAsync(string name, string laboratory, int pageNumber, int pageSize)
        {
            var query = _context.Medicines.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(m => m.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(laboratory))
            {
                query = query.Where(m => m.Laboratory.Contains(laboratory));
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Update(Medicine data)
        {
            _context.Medicines.Update(data);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountMedicinesAsync(string name, string laboratory)
        {
            var query = _context.Medicines.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(m => m.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(laboratory))
            {
                query = query.Where(m => m.Laboratory.Contains(laboratory));
            }

            return await query.CountAsync();
        }
    }
}
