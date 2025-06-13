using CsvHelper;
using Medicines.Application.Commands.Ingestion;
using Medicines.Application.DTOs.Ingestion;
using Medicines.Core.DTOs;
using Medicines.Core.Entities;
using Medicines.Core.Enums;
using Medicines.Core.Repositories;
using Serilog;
using System.Globalization;
using System.Text.Json;

namespace Medicines.Application.Handlers.Ingestion
{
    public class UploadDataCommandHandler
    {
        private readonly IIngestionProcessRepository _ingestionProcessRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IAuditRepository _auditRepository;

        public UploadDataCommandHandler(
            IIngestionProcessRepository ingestionProcessRepository,
            IMedicineRepository medicineRepository,
            IAuditRepository auditRepository)
        {
            _ingestionProcessRepository = ingestionProcessRepository;
            _medicineRepository = medicineRepository;
            _auditRepository = auditRepository;
        }

        public async Task<IngestionResultDto> Handle(UploadDataCommand request)
        {
            Guid ingestionId = Guid.NewGuid();
            var ingestionProcess = new IngestionProcess
            {
                Id = ingestionId,
                FileName = request.FileName,
                FileType = request.ContentType.Contains("csv") ? "CSV" : "JSON",
                Status = IngestionStatus.Pending,
                StartedAt = DateTime.UtcNow
            };

            await _ingestionProcessRepository.Add(ingestionProcess);

            try
            {
                ingestionProcess.Status = IngestionStatus.Processing;
                await _ingestionProcessRepository.Update(ingestionProcess);

                List<MedicineCsvDto> csvMedicines = new List<MedicineCsvDto>();
                List<MedicineJsonDto> jsonMedicines = new List<MedicineJsonDto>();
                List<Medicine> medicinesToProcess = new List<Medicine>();
                List<string> processingErrors = new List<string>();

                if (ingestionProcess.FileType == "CSV")
                {
                    using (var reader = new StreamReader(new MemoryStream(request.FileContent)))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csvMedicines = csv.GetRecords<MedicineCsvDto>().ToList();
                        ingestionProcess.TotalRecords = csvMedicines.Count;
                    }
                }
                else if (ingestionProcess.FileType == "JSON")
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var jsonContent = JsonSerializer.Deserialize<JsonIngestionInput>(request.FileContent, options);
                    jsonMedicines = jsonContent?.Medicines ?? new List<MedicineJsonDto>();
                    ingestionProcess.TotalRecords = jsonMedicines.Count;
                }
                else
                {
                    throw new InvalidOperationException("Unsupported file type.");
                }

                int processedCount = 0;
                int validCount = 0;
                int invalidCount = 0;

                foreach (var csvMed in csvMedicines)
                {
                    processedCount++;
                    var (medicine, isValid, errors) = ValidateAndMapMedicine(csvMed, ingestionProcess.FileName, ingestionProcess.StartedAt);
                    if (isValid)
                    {
                        medicinesToProcess.Add(medicine);
                        validCount++;
                    }
                    else
                    {
                        invalidCount++;
                        processingErrors.Add($"Record {processedCount} (CSV): {string.Join(", ", errors)}");
                    }
                }

                foreach (var jsonMed in jsonMedicines)
                {
                    processedCount++;
                    var (medicine, isValid, errors) = ValidateAndMapMedicine(jsonMed, ingestionProcess.FileName, ingestionProcess.StartedAt);
                    if (isValid)
                    {
                        medicinesToProcess.Add(medicine);
                        validCount++;
                    }
                    else
                    {
                        invalidCount++;
                        processingErrors.Add($"Record {processedCount} (JSON): {string.Join(", ", errors)}");
                    }
                }

                ingestionProcess.ProcessedRecords = processedCount;
                ingestionProcess.ValidRecords = validCount;
                ingestionProcess.InvalidRecords = invalidCount;

                foreach (var newMedicine in medicinesToProcess)
                {
                    var existingMedicine = await _medicineRepository.GetByCode(newMedicine.Code);
                    List<AuditEntry> auditEntries = new List<AuditEntry>();

                    if (existingMedicine == null)
                    {
                        await _medicineRepository.Add(newMedicine);
                    }
                    else
                    {
                        if (existingMedicine.Name != newMedicine.Name) auditEntries.Add(CreateAuditEntry(existingMedicine.Id, "Name", existingMedicine.Name, newMedicine.Name, newMedicine.SourceFileName, newMedicine.SourceFileTimestamp));
                        if (existingMedicine.Laboratory != newMedicine.Laboratory) auditEntries.Add(CreateAuditEntry(existingMedicine.Id, "Laboratory", existingMedicine.Laboratory, newMedicine.Laboratory, newMedicine.SourceFileName, newMedicine.SourceFileTimestamp));
                        if (existingMedicine.ActiveIngredient != newMedicine.ActiveIngredient) auditEntries.Add(CreateAuditEntry(existingMedicine.Id, "ActiveIngredient", existingMedicine.ActiveIngredient, newMedicine.ActiveIngredient, newMedicine.SourceFileName, newMedicine.SourceFileTimestamp));
                        if (existingMedicine.Concentration != newMedicine.Concentration) auditEntries.Add(CreateAuditEntry(existingMedicine.Id, "Concentration", existingMedicine.Concentration, newMedicine.Concentration, newMedicine.SourceFileName, newMedicine.SourceFileTimestamp));
                        if (existingMedicine.Presentation != newMedicine.Presentation) auditEntries.Add(CreateAuditEntry(existingMedicine.Id, "Presentation", existingMedicine.Presentation, newMedicine.Presentation, newMedicine.SourceFileName, newMedicine.SourceFileTimestamp));
                        if (existingMedicine.ExpirationDate != newMedicine.ExpirationDate) auditEntries.Add(CreateAuditEntry(existingMedicine.Id, "ExpirationDate", existingMedicine.ExpirationDate.ToString("yyyy-MM-dd"), newMedicine.ExpirationDate.ToString("yyyy-MM-dd"), newMedicine.SourceFileName, newMedicine.SourceFileTimestamp));

                        existingMedicine.Name = newMedicine.Name;
                        existingMedicine.Laboratory = newMedicine.Laboratory;
                        existingMedicine.ActiveIngredient = newMedicine.ActiveIngredient;
                        existingMedicine.Concentration = newMedicine.Concentration;
                        existingMedicine.Presentation = newMedicine.Presentation;
                        existingMedicine.ExpirationDate = newMedicine.ExpirationDate;
                        existingMedicine.LastModifiedAt = DateTime.UtcNow;
                        existingMedicine.SourceFileName = newMedicine.SourceFileName;
                        existingMedicine.SourceFileTimestamp = newMedicine.SourceFileTimestamp;

                        await _medicineRepository.Update(existingMedicine);

                        if (auditEntries.Any())
                        {
                            await _auditRepository.AddRange(auditEntries);
                        }
                    }
                }

                ingestionProcess.Status = IngestionStatus.Completed;
                ingestionProcess.CompletedAt = DateTime.UtcNow;
                if (processingErrors.Any())
                {
                    ingestionProcess.ErrorDetails = string.Join("\n", processingErrors);
                }
                await _ingestionProcessRepository.Update(ingestionProcess);

                return new IngestionResultDto
                {
                    IngestionId = ingestionId,
                    Status = IngestionStatus.Completed,
                    Message = "Data ingestion initiated and processed successfully."
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during data ingestion for file {FileName} (ID: {IngestionId})", request.FileName, ingestionId);
                ingestionProcess.Status = IngestionStatus.Failed;
                ingestionProcess.CompletedAt = DateTime.UtcNow;
                ingestionProcess.ErrorDetails = ex.Message;
                await _ingestionProcessRepository.Update(ingestionProcess);

                return new IngestionResultDto
                {
                    IngestionId = ingestionId,
                    Status = IngestionStatus.Failed,
                    Message = "Data ingestion failed.",
                    ErrorDetails = ex.Message
                };
            }
        }

        private (Medicine, bool, List<string>) ValidateAndMapMedicine(dynamic data, string sourceFileName, DateTime sourceFileTimestamp)
        {
            List<string> errors = new List<string>();
            Guid medicineId = Guid.NewGuid();

            var code = "";
            var name = "";
            var laboratory = "";
            var activeIngredient = "";
            var concentration = "";
            var presentation = "";
            var expirationDateStr = "";
            var expirationDate = DateTime.MinValue;

            try
            {

                code = data.Code ?? data.medicine_code?.ToString();
                name = data.Name ?? data.medicine_name?.ToString();
                laboratory = data.Laboratory ?? data.laboratory?.ToString();
                activeIngredient = data.ActiveIngredient ?? data.active_ingredient?.ToString();
                concentration = data.Concentration ?? data.concentration?.ToString();
                presentation = data.Presentation ?? data.presentation?.ToString();
                expirationDateStr = data.ExpirationDate ?? data.expiration_date?.ToString();

                if (string.IsNullOrWhiteSpace(code)) errors.Add("Code is required.");
                if (string.IsNullOrWhiteSpace(name)) errors.Add("Name is required.");
                if (string.IsNullOrWhiteSpace(laboratory)) errors.Add("Laboratory is required.");
                if (string.IsNullOrWhiteSpace(activeIngredient)) errors.Add("ActiveIngredient is required.");

                if (!DateTime.TryParse(expirationDateStr, out expirationDate))
                {
                    errors.Add("Invalid ExpirationDate format.");
                }
                else if (expirationDate < DateTime.Today)
                {
                    errors.Add("ExpirationDate must be in the future.");
                }

                if (!string.IsNullOrWhiteSpace(concentration) && !System.Text.RegularExpressions.Regex.IsMatch(concentration, @"^\d+(\.\d+)?(mg|ml|UI|mcg|milligramos|ml/dl|unidad|g|kg|L|µg|ug)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    errors.Add("Concentration format is invalid.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error during data Validation for medicineId: {medicineId} - code: {code} - sourceFileName: {sourceFileName}");
                errors.Add(ex.Message);
            }

            if (errors.Any())
            {
                return (null, false, errors);
            }

            return (new Medicine
            {
                Id = medicineId,
                Code = code,
                Name = name,
                Laboratory = laboratory,
                ActiveIngredient = activeIngredient,
                Concentration = concentration,
                Presentation = presentation,
                ExpirationDate = expirationDate,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                SourceFileName = sourceFileName,
                SourceFileTimestamp = sourceFileTimestamp
            }, true, new List<string>());
        }

        private AuditEntry CreateAuditEntry(Guid medicineId, string fieldName, string oldValue, string newValue, string sourceFile, DateTime sourceFileTimestamp)
        {
            return new AuditEntry
            {
                Id = Guid.NewGuid(),
                EntityId = medicineId,
                EntityType = "Medicine",
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = "Data Ingestion System",
                SourceFile = sourceFile,
                SourceFileTimestamp = sourceFileTimestamp
            };
        }
    }
}
