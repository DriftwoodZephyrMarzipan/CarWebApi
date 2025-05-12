using CarWebApi.Data.Repositories;
using CarWebApi.EvDataModels.Models;
using CarWebApi.EvDataModels.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CarWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MakesController : BaseController
    {
        private readonly ILogger<MakesController> _logger;
        private readonly ICarRepository _carRepository;


        public MakesController(ICarRepository carRepository, ILogger<MakesController> logger)
        {
            _carRepository = carRepository;
            _logger = logger;
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IdList))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var idList = await _carRepository.GetMakeIdListAsync();
            return Ok(idList);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Make))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("There are no Makes with an Id less than or equal to zero");
            }
            var make = await _carRepository.GetMakeByIdAsync(id);

            if (make == null)
            {
                return NotFound($"There are no Makes with an Id of {id}");
            }
            return Ok(make);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<Make>))]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allMakes = await _carRepository.GetAllMakesAsync();
            return Ok(allMakes);
        }

        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreatedAtActionResult))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Make make)
        {
            if (make == null)
            {
                return BadRequest("Make cannot be null");
            }
            if(make.Id != 0)
            {
                return BadRequest("Make Id must be zero");
            }
            if (string.IsNullOrWhiteSpace(make.Manufacturer))
            {
                return BadRequest("Make name cannot be null or empty");
            }
            try
            {
                var result = await _carRepository.CreateMakeAsync(make);
                if (result)
                {
                    return CreatedAtAction(nameof(Get), new { id = make.Id }, make);
                }
                return BadRequest("Failed to create duplicate Make");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Make");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the Make");
            }
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Make))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Make make)
        {
            if (make == null)
            {
                return BadRequest("Make cannot be null");
            }
            if (make.Id == 0)
            {
                return BadRequest("Make Id must not be zero");
            }
            if (string.IsNullOrWhiteSpace(make.Manufacturer))
            {
                return BadRequest("Make name cannot be null or empty");
            }
            try
            {
                var result = await _carRepository.UpdateMakeAsync(make);
                if (result)
                {
                    return Ok(make);
                }
                return BadRequest("Failed to create duplicate Make");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Make");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the Make");
            }
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Make))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest("Make Id must not be zero");
            }
            try
            {
                var result = await _carRepository.DeleteMakeAsync(id);
                if (result)
                {
                    return Ok($"Deleted Make with id {id}");
                }
                return BadRequest($"Failed to create delete Make {id} - does not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delete Make");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while deleting the Make");
            }
        }
    }
}
