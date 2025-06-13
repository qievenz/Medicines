using Medicines.Core.DTOs.Ingestion;
using Medicines.Core.Entities;
using System.Text.Json;

namespace Medicines.Core.Services
{
    public interface IMedicineValidationService
    {
        (Medicine medicine, bool isValid, List<string> errors) ValidateAndMapMedicineFromCsv(MedicineCsvDto csvData, string sourceFileName, DateTime sourceFileTimestamp);
        (Medicine medicine, bool isValid, List<string> errors) ValidateAndMapMedicineFromJson(JsonElement jsonData, string sourceFileName, DateTime sourceFileTimestamp);
    }
}
