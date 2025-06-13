using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Medicines.Core.Entities;
using Medicines.Core.Services;
using Serilog;

namespace Medicines.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Medicine>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll()
        {
            Log.Information("GET /api/Data - Request to get all data records.");
            var datas = await _dataService.GetAllDatasAsync();

            return Ok(datas);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Medicine))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetById(int id)
        {
            Log.Information("GET /api/Data/{Id} - Request to get data record by ID: {Id}.", id);
            var data = await _dataService.GetDataByIdAsync(id);

            return Ok(data);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Medicine))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create([FromBody] Medicine data)
        {
            Log.Information("POST /api/Data - Request to create a new data record.");
            if (!ModelState.IsValid)
            {
                Log.Warning("POST /api/Data - Model state is invalid.");

                return BadRequest(ModelState);
            }

            var createdData = await _dataService.CreateDataAsync(data);
            Log.Information("POST /api/Data - Data record with ID {Id} created successfully.", createdData.Id);

            return CreatedAtAction(nameof(GetById), new { id = createdData.Id }, createdData);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Update(int id, [FromBody] Medicine data)
        {
            Log.Information("PUT /api/Data/{Id} - Request to update data record with ID: {Id}.", id);

            if (!ModelState.IsValid)
            {
                Log.Warning("PUT /api/Data/{Id} - Model state is invalid.");

                return BadRequest(ModelState);
            }

            await _dataService.UpdateDataAsync(id, data);
            Log.Information("PUT /api/Data/{Id} - Data record with ID {Id} updated successfully.", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            Log.Information("DELETE /api/Data/{Id} - Request to delete data record with ID: {Id}.", id);
            await _dataService.DeleteDataAsync(id);
            Log.Information("DELETE /api/Data/{Id} - Data record with ID {Id} deleted successfully.", id);

            return NoContent();
        }
    }
}
