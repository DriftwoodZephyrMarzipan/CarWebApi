using CarWebApi.EvDataModels.Models;
using CarWebApi.Utils;
using CarWebApi.EvDataModels.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CarWebApi.Services;

namespace CarWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelemetryController : BaseController
    {
        private readonly ILogger<EvTypesController> _logger;
        private readonly IRequestTelemetryService _requestTelemetry;
        private readonly IDatabaseTelemetryService _databaseTelemetry;

        public TelemetryController(IRequestTelemetryService requestTelemetry,
                                   IDatabaseTelemetryService databaseTelemetry,
                                   ILogger<EvTypesController> logger)
        {
            _logger = logger;
            _requestTelemetry = requestTelemetry;
            _databaseTelemetry = databaseTelemetry; 
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IDictionary<string, IDictionary<string, int>>))]
        [HttpGet("methodCalls")]
        public IActionResult GetControllerMethodCalls()
        {
            var stats = _requestTelemetry.GetStats();
            return Ok(stats);
        }

        [HttpGet("queries")]
        public IActionResult GetLastQueries()
        {
            var lastQueries = _databaseTelemetry.GetLastQueries();
            return Ok(lastQueries);
        }
    }
}
