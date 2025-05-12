using CarWebApi.Data.Repositories;
using CarWebApi.EvDataModels.Models;
using CarWebApi.EvDataModels.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CarWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarsController : BaseController
    {
        private readonly ILogger<CarsController> _logger;
        private readonly ICarRepository _carRepository;

        public CarsController(ICarRepository carRepository, ILogger<CarsController> logger)
        {
            _carRepository = carRepository;
            _logger = logger;
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IdList))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var idList = await _carRepository.GetCarIdListAsync();
            return Ok(idList);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Car))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("There are no Cars with an Id less than or equal to zero");
            }
            var car = await _carRepository.GetCarByIdAsync(id);

            if (car == null)
            {
                return NotFound($"There are no Cars with an Id of {id}");
            }
            return Ok(car);
        }

        // This method does not exist because we don't want to return 97 megs of car data
        /*
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<Car>))]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allCars = await _carRepository.GetAllCarsAsync();
            return Ok(allCars);
        }
        */

        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreatedAtActionResult))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Car car)
        {
            if (car == null)
            {
                return BadRequest("Car cannot be null");
            }
            if(car.Id != 0)
            {
                return BadRequest("Car Id must be zero");
            }
            if (car.Uuid == Guid.Empty)
            {
                return BadRequest("Car uuid cannot be empty");
            }
            try
            {
                var result = await _carRepository.CreateCarAsync(car);
                if (result)
                {
                    return CreatedAtAction(nameof(Get), new { id = car.Id }, car);
                }
                return BadRequest("Failed to create duplicate Car");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Car");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the Car");
            }
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Car))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Car car)
        {
            if (car == null)
            {
                return BadRequest("Car cannot be null");
            }
            if (car.Id == 0)
            {
                return BadRequest("Car Id must not be zero");
            }
            if (car.Uuid == Guid.Empty)
            {
                return BadRequest("Car uuid cannot be empty");
            }
            try
            {
                var result = await _carRepository.UpdateCarAsync(car);
                if (result)
                {
                    return Ok(car);
                }
                return BadRequest("Failed to update duplicate Car");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error update Car");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while updating the Car");
            }
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Car))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest("Car Id must not be zero");
            }
            try
            {
                var result = await _carRepository.DeleteCarAsync(id);
                if (result)
                {
                    return Ok($"Deleted Car with id {id}");
                }
                return BadRequest($"Failed to create delete Car {id} - does not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delete Car");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while deleting the Car");
            }
        }
    }
}
