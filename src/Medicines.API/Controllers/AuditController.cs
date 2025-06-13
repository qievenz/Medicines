using Medicines.Application.Handlers.Audit;
using Medicines.Application.Queries.Audit;
using Medicines.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicines.API.Controllers
{
    [ApiController]
    [Route("api/v1/audit")]
    [Authorize]
    public class AuditController : ControllerBase
    {
        private readonly GetMedicineHistoryQueryHandler _getMedicineHistoryQueryHandler;

        public AuditController(GetMedicineHistoryQueryHandler getMedicineHistoryQueryHandler)
        {
            _getMedicineHistoryQueryHandler = getMedicineHistoryQueryHandler;
        }

        [HttpGet("{medicineId}/history")]
        public async Task<ActionResult<IEnumerable<AuditEntryDto>>> GetMedicineHistory(Guid medicineId)
        {
            var query = new GetMedicineHistoryQuery { MedicineId = medicineId };
            var result = await _getMedicineHistoryQueryHandler.Handle(query);

            if (result == null || !result.Any())
            {
                return NotFound("No audit history found for the given medicine ID.");
            }

            return Ok(result);
        }
    }
}
