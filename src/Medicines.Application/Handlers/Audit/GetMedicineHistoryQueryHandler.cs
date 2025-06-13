using Medicines.Application.Queries.Audit;
using Medicines.Core.DTOs;
using Medicines.Core.Repositories;

namespace Medicines.Application.Handlers.Audit
{
    public class GetMedicineHistoryQueryHandler
    {
        private readonly IAuditRepository _auditRepository;

        public GetMedicineHistoryQueryHandler(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        public async Task<IEnumerable<AuditEntryDto>> Handle(GetMedicineHistoryQuery request)
        {
            var auditEntries = await _auditRepository.GetHistoryByEntityId(request.MedicineId);

            return auditEntries.Select(a => new AuditEntryDto
            {
                Id = a.Id,
                EntityId = a.EntityId,
                FieldName = a.FieldName,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                ChangedAt = a.ChangedAt,
                ChangedBy = a.ChangedBy,
                SourceFile = a.SourceFile,
                SourceFileTimestamp = a.SourceFileTimestamp
            }).ToList();
        }
    }
}
