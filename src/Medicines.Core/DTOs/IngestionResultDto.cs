using Medicines.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Core.DTOs
{
    public class IngestionResultDto
    {
        public Guid IngestionId { get; set; }
        public IngestionStatus Status { get; set; }
        public string Message { get; set; }
        public string ErrorDetails { get; set; }
    }
}
