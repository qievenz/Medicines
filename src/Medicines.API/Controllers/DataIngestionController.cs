using Medicines.Application.Commands.Ingestion;
using Medicines.Application.Handlers.Ingestion;
using Medicines.Application.Queries.Ingestion;
using Medicines.Core.DTOs;
using Medicines.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicines.API.Controllers
{
    [ApiController]
    [Route("api/v1/data-ingestion")]
    [Authorize]
    public class DataIngestionController : ControllerBase
    {
        private readonly UploadDataCommandHandler _uploadDataCommandHandler;
        private readonly GetIngestionStatusQueryHandler _getIngestionStatusQueryHandler;

        public DataIngestionController(
            UploadDataCommandHandler uploadDataCommandHandler,
            GetIngestionStatusQueryHandler getIngestionStatusQueryHandler)
        {
            _uploadDataCommandHandler = uploadDataCommandHandler;
            _getIngestionStatusQueryHandler = getIngestionStatusQueryHandler;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or not provided.");
            }

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var command = new UploadDataCommand
                {
                    FileName = file.FileName,
                    FileContent = ms.ToArray(),
                    ContentType = file.ContentType
                };

                var result = await _uploadDataCommandHandler.Handle(command);

                if (result.Status == IngestionStatus.Failed)
                {
                    return StatusCode(500, result);
                }

                return Ok(result);
            }
        }

        [HttpGet("{ingestionId}/status")]
        public async Task<ActionResult<IngestionResultDto>> GetStatus(Guid ingestionId)
        {
            var query = new GetIngestionStatusQuery { IngestionId = ingestionId };
            var result = await _getIngestionStatusQueryHandler.Handle(query);

            if (result.Status == IngestionStatus.Failed && result.Message == "Ingestion process not found.")
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
