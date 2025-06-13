using Medicines.Core.Entities;
using Medicines.Core.Repositories;
using Medicines.Infrastructure.Persistence;

namespace Medicines.Infrastructure.Repositories
{
    public class IngestionProcessRepository : IIngestionProcessRepository
    {
        private readonly ApplicationDbContext _context;

        public IngestionProcessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(IngestionProcess process)
        {
            _context.IngestionProcesses.Add(process);
            await _context.SaveChangesAsync();
        }

        public async Task<IngestionProcess> GetById(Guid id)
        {
            return await _context.IngestionProcesses.FindAsync(id);
        }

        public async Task Update(IngestionProcess process)
        {
            _context.IngestionProcesses.Update(process);
            await _context.SaveChangesAsync();
        }
    }
}
