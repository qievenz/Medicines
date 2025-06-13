using CsvHelper;
using CsvHelper.Configuration;
using Medicines.Application.Commands.Ingestion;
using Medicines.Core.DTOs;
using Medicines.Core.DTOs.Ingestion;
using Medicines.Core.Entities;
using Medicines.Core.Enums;
using Medicines.Core.Repositories;
using Medicines.Core.Services;
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
        private readonly IMedicineValidationService _medicineValidationService;

        public UploadDataCommandHandler(
            IIngestionProcessRepository ingestionProcessRepository,
            IMedicineRepository medicineRepository,
            IAuditRepository auditRepository,
            IMedicineValidationService medicineValidationService)
        {
            _ingestionProcessRepository = ingestionProcessRepository;
            _medicineRepository = medicineRepository;
            _auditRepository = auditRepository;
            _medicineValidationService = medicineValidationService;
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
                    var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        MissingFieldFound = null,
                        HeaderValidated = null
                    };
                    using (var reader = new StreamReader(new MemoryStream(request.FileContent)))
                    using (var csv = new CsvReader(reader, csvConfig))
                    {
                        var records = csv.GetRecords<MedicineCsvDto>().ToList();
                        ingestionProcess.TotalRecords = records.Count;

                        int recordIndex = 0;
                        foreach (var csvMed in records)
                        {
                            recordIndex++;
                            var (medicine, isValid, errors) = _medicineValidationService.ValidateAndMapMedicineFromCsv(csvMed, ingestionProcess.FileName, ingestionProcess.StartedAt);
                            if (isValid)
                            {
                                medicinesToProcess.Add(medicine);
                            }
                            else
                            {
                                processingErrors.Add($"Record CSV {recordIndex}: {string.Join(", ", errors)}");
                            }
                        }
                    }
                }
                else if (ingestionProcess.FileType == "JSON")
                {
                    var jsonDoc = JsonDocument.Parse(request.FileContent);
                    var jsonContent = jsonDoc.RootElement;

                    if (jsonContent.TryGetProperty("medicines", out JsonElement medicinesElement) && medicinesElement.ValueKind == JsonValueKind.Array)
                    {
                        ingestionProcess.TotalRecords = medicinesElement.EnumerateArray().Count();
                        int recordIndex = 0;
                        foreach (var jsonMedElement in medicinesElement.EnumerateArray())
                        {
                            recordIndex++;
                            var (medicine, isValid, errors) = _medicineValidationService.ValidateAndMapMedicineFromJson(jsonMedElement, ingestionProcess.FileName, ingestionProcess.StartedAt);
                            if (isValid)
                            {
                                medicinesToProcess.Add(medicine);
                            }
                            else
                            {
                                processingErrors.Add($"Record JSON {recordIndex}: {string.Join(", ", errors)}");
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("JSON file does not contain a 'medicines' array or is malformed.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unsupported file type.");
                }
                ingestionProcess.ValidRecords = medicinesToProcess.Count;
                ingestionProcess.InvalidRecords = processingErrors.Count;
                ingestionProcess.ProcessedRecords = ingestionProcess.ValidRecords + ingestionProcess.InvalidRecords;


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
                    Message = "Data ingestion processed.",
                    ErrorDetails = ingestionProcess.ErrorDetails
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
