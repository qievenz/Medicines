﻿namespace Medicines.Core.Entities
{
    public class AuditEntry
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public string EntityType { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public string SourceFile { get; set; }
        public DateTime SourceFileTimestamp { get; set; }
    }
}
