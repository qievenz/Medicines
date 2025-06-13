using FluentValidation;
using Medicines.Core.DTOs.Ingestion;
using Medicines.Core.Entities;
using Medicines.Core.Services;
using System.Globalization;
using System.Text.Json;

namespace Medicines.Application.Services
{
    public class MedicineValidationService : IMedicineValidationService
    {
        private readonly IValidator<MedicineCsvDto> _csvValidator;
        private readonly IValidator<MedicineJsonDto> _jsonValidator;

        public MedicineValidationService(
            IValidator<MedicineCsvDto> csvValidator,
            IValidator<MedicineJsonDto> jsonValidator)
        {
            _csvValidator = csvValidator;
            _jsonValidator = jsonValidator;
        }

        public (Medicine medicine, bool isValid, List<string> errors) ValidateAndMapMedicineFromCsv(MedicineCsvDto csvData, string sourceFileName, DateTime sourceFileTimestamp)
        {
            var validationResult = _csvValidator.Validate(csvData);
            if (!validationResult.IsValid)
            {
                return (null, false, validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            DateTime expirationDate = DateTime.Parse(csvData.expiration_date, CultureInfo.InvariantCulture);

            var medicine = new Medicine
            {
                Id = Guid.NewGuid(),
                Code = csvData.medicine_code,
                Name = csvData.medicine_name,
                Laboratory = csvData.laboratory,
                ActiveIngredient = csvData.active_ingredient,
                Concentration = csvData.concentration,
                Presentation = csvData.presentation,
                ExpirationDate = expirationDate,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                SourceFileName = sourceFileName,
                SourceFileTimestamp = sourceFileTimestamp
            };

            return (medicine, true, new List<string>());
        }

        public (Medicine medicine, bool isValid, List<string> errors) ValidateAndMapMedicineFromJson(JsonElement jsonData, string sourceFileName, DateTime sourceFileTimestamp)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            MedicineJsonDto jsonDto = new MedicineJsonDto();

            jsonDto.Code = GetPropertyString(jsonData, "code");
            jsonDto.Name = GetPropertyString(jsonData, "name");
            jsonDto.Laboratory = GetPropertyString(jsonData, "laboratory");
            jsonDto.ActiveIngredient = GetPropertyString(jsonData, "activeIngredient", "active_ingredient");
            jsonDto.Concentration = GetPropertyString(jsonData, "concentration");
            jsonDto.Presentation = GetPropertyString(jsonData, "presentation");
            jsonDto.ExpirationDate = GetPropertyString(jsonData, "expirationDate");

            var validationResult = _jsonValidator.Validate(jsonDto);
            if (!validationResult.IsValid)
            {
                return (null, false, validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            DateTime expirationDate = DateTime.Parse(jsonDto.ExpirationDate, CultureInfo.InvariantCulture);

            var medicine = new Medicine
            {
                Id = Guid.NewGuid(),
                Code = jsonDto.Code,
                Name = jsonDto.Name,
                Laboratory = jsonDto.Laboratory,
                ActiveIngredient = jsonDto.ActiveIngredient,
                Concentration = jsonDto.Concentration,
                Presentation = jsonDto.Presentation,
                ExpirationDate = expirationDate,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                SourceFileName = sourceFileName,
                SourceFileTimestamp = sourceFileTimestamp
            };

            return (medicine, true, new List<string>());
        }

        private string GetPropertyString(JsonElement element, params string[] propertyNames)
        {
            foreach (var propName in propertyNames)
            {
                if (element.TryGetProperty(propName, out JsonElement property) && property.ValueKind == JsonValueKind.String)
                {
                    return property.GetString();
                }
            }
            return null;
        }
    }
}
