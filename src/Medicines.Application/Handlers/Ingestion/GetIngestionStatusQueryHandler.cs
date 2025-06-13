using Medicines.Application.Queries.Ingestion;
using Medicines.Core.DTOs;
using Medicines.Core.Enums;
using Medicines.Core.Repositories;

namespace Medicines.Application.Handlers.Ingestion
{
    public class GetIngestionStatusQueryHandler
    {
        private readonly IIngestionProcessRepository _ingestionProcessRepository;

        public GetIngestionStatusQueryHandler(IIngestionProcessRepository ingestionProcessRepository)
        {
            _ingestionProcessRepository = ingestionProcessRepository;
        }

        public async Task<IngestionResultDto> Handle(GetIngestionStatusQuery request)
        {
            var process = await _ingestionProcessRepository.GetById(request.IngestionId);

            if (process == null)
            {
                return new IngestionResultDto
                {
                    IngestionId = request.IngestionId,
                    Status = IngestionStatus.Failed,
                    Message = "Ingestion process not found."
                };
            }

            return new IngestionResultDto
            {
                IngestionId = process.Id,
                Status = process.Status,
                Message = $"Process {process.Status}.",
                ErrorDetails = process.ErrorDetails
            };
        }
    }
}
