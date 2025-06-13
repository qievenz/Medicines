namespace Medicines.Core.DTOs.Ingestion
{
    public class MedicineJsonDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Laboratory { get; set; }
        public string ActiveIngredient { get; set; }
        public string Concentration { get; set; }
        public string Presentation { get; set; }
        public string ExpirationDate { get; set; }
    }
}
