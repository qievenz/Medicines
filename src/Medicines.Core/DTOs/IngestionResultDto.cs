using Medicines.Core.Enums;

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
