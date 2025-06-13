using Medicines.Application.Handlers.Medicines;
using Medicines.Application.Queries.Medicines;
using Medicines.Core.DTOs;
using Medicines.Core.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicines.API.Controllers
{
    [ApiController]
    [Route("api/v1/medicines")]
    [Authorize]
    public class MedicinesController : ControllerBase
    {
        private readonly GetPagedMedicinesQueryHandler _getPagedMedicinesQueryHandler;

        public MedicinesController(GetPagedMedicinesQueryHandler getPagedMedicinesQueryHandler)
        {
            _getPagedMedicinesQueryHandler = getPagedMedicinesQueryHandler;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<MedicineDto>>> GetMedicines(
            [FromQuery] string name = "",
            [FromQuery] string laboratory = "",
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var query = new GetPagedMedicinesQuery
            {
                Name = name,
                Laboratory = laboratory,
                PageNumber = page,
                PageSize = size
            };

            var result = await _getPagedMedicinesQueryHandler.Handle(query);

            return Ok(result);
        }
    }
}
