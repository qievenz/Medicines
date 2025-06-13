using Medicines.Core.Entities;
using Medicines.Core.Repositories;
using Medicines.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Infrastructure.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(AuditEntry auditEntry)
        {
            _context.AuditEntries.Add(auditEntry);
            await _context.SaveChangesAsync();
        }

        public async Task AddRange(IEnumerable<AuditEntry> auditEntries)
        {
            _context.AuditEntries.AddRange(auditEntries);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditEntry>> GetHistoryByEntityId(Guid entityId)
        {
            return await _context.AuditEntries
               .Where(a => a.EntityId == entityId)
               .OrderByDescending(a => a.ChangedAt)
               .ToListAsync();
        }
    }
}
