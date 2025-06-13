using Medicines.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Core.Repositories
{
    public interface IAuditRepository
    {
        Task Add(AuditEntry auditEntry);
        Task AddRange(IEnumerable<AuditEntry> auditEntries);
        Task<IEnumerable<AuditEntry>> GetHistoryByEntityId(Guid entityId);
    }
}
