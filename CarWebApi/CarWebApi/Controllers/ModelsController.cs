using CarWebApi.Data.Repositories;
using CarWebApi.EvDataModels.Models;
using CarWebApi.EvDataModels.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CarWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelsController : BaseController
    {
        private readonly ILogger<ModelsController> _logger;
        private readonly ICarRepository _carRepository;

        public ModelsController(ICarRepository carRepository, ILogger<ModelsController> logger)
        {
            _carRepository = carRepository;
            _logger = logger;
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IdList))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var idList = await _carRepository.GetModelIdListAsync();
            return Ok(idList);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Model))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("There are no Models with an Id less than or equal to zero");
            }
            var model = await _carRepository.GetModelByIdAsync(id);

            if (model == null)
            {
                return NotFound($"There are no Models with an Id of {id}");
            }
            return Ok(model);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<Model>))]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allModels = await _carRepository.GetAllModelsAsync();
            return Ok(allModels);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<Model>))]
        [HttpGet("all/{make_id:int}")]
        public async Task<IActionResult> GetAll(int make_id)
        {
            var allModels = await _carRepository.GetModelsByMakeIdAsync(make_id);
            return Ok(allModels);
        }


        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreatedAtActionResult))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Model model)
        {
            if (model == null)
            {
                return BadRequest("Model cannot be null");
            }
            if(model.Id != 0)
            {
                return BadRequest("Model Id must be zero");
            }
            if (string.IsNullOrWhiteSpace(model.ModelName))
            {
                return BadRequest("Model name cannot be null or empty");
            }
            try
            {
                var result = await _carRepository.CreateModelAsync(model);
                if (result)
                {
                    return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
                }
                return BadRequest("Failed to create duplicate Model");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Model");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the Model");
            }
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Model))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model model)
        {
            if (model == null)
            {
                return BadRequest("Model cannot be null");
            }
            if (model.Id == 0)
            {
                return BadRequest("Model Id must not be zero");
            }
            if (string.IsNullOrWhiteSpace(model.ModelName))
            {
                return BadRequest("Model name cannot be null or empty");
            }
            try
            {
                var result = await _carRepository.UpdateModelAsync(model);
                if (result)
                {
                    return Ok(model);
                }
                return BadRequest("Failed to create duplicate Model");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Model");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the Model");
            }
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Model))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest("Model Id must not be zero");
            }
            try
            {
                var result = await _carRepository.DeleteModelAsync(id);
                if (result)
                {
                    return Ok($"Deleted Model with id {id}");
                }
                return BadRequest($"Failed to create delete Model {id} - does not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delete Model");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while deleting the Model");
            }
        }
    }
}
