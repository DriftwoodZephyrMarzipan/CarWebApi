using CarWebApi.EvDataModels.Models;
using CarWebApi.Utils;
using CarWebApi.EvDataModels.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CarWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EvTypesController : BaseController
    {
        private readonly ILogger<EvTypesController> _logger;


        public EvTypesController(ILogger<EvTypesController> logger)
        {
            _logger = logger;
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<EnumIdentifier>))]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok( EnumHelper.GetEnumIdentifiers<EvType>());
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(EnumIdentifier))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            if (id < 0)
            {
                return BadRequest("There are no EvTypes with an Id less than zero");
            }

            var enumIdentifier = EnumHelper.GetEnumIdentifierById<EvType>(id);

            if(enumIdentifier == null)
            {
                return NotFound($"There are no EvTypes with an Id of {id}");
            }
            return Ok(enumIdentifier);
        }
    }
}
