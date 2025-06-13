using Medicines.Core.Entities;

namespace Medicines.Core.Repositories
{
    public interface IAuditRepository
    {
        Task Add(AuditEntry auditEntry);
        Task AddRange(IEnumerable<AuditEntry> auditEntries);
        Task<IEnumerable<AuditEntry>> GetHistoryByEntityId(Guid entityId);
    }
}
