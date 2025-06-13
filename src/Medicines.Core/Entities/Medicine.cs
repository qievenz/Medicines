namespace Medicines.Core.Entities
{
    public class Medicine
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Laboratory { get; set; }
        public string ActiveIngredient { get; set; }
        public string Concentration { get; set; }
        public string Presentation { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string SourceFileName { get; set; }
        public DateTime SourceFileTimestamp { get; set; }
    }
}
