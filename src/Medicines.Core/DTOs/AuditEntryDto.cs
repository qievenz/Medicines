using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Core.DTOs
{
    public class AuditEntryDto
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public string SourceFile { get; set; }
        public DateTime SourceFileTimestamp { get; set; }
    }
}
