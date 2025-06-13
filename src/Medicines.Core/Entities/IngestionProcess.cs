using Medicines.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Core.Entities
{
    public class IngestionProcess
    {
        public Guid Id { get; set; }
        public IngestionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string ErrorDetails { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
    }
}
