namespace Medicines.Application.Commands.Ingestion
{
    public class UploadDataCommand
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
    }
}
