namespace Medicines.Application.DTOs.Ingestion
{
    public class MedicineCsvDto
    {
        public string medicine_code { get; set; }
        public string medicine_name { get; set; }
        public string laboratory { get; set; }
        public string active_ingredient { get; set; }
        public string concentration { get; set; }
        public string presentation { get; set; }
        public string expiration_date { get; set; }
    }
}
