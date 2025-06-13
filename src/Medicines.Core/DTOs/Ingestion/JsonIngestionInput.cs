namespace Medicines.Core.DTOs.Ingestion
{
    public class JsonIngestionInput
    {
        public string Provider { get; set; }
        public DateTime Timestamp { get; set; }
        public List<MedicineJsonDto> Medicines { get; set; }
    }
}
