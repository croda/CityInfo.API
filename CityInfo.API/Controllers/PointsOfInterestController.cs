using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            // HttpContext.RequestServices.GetService(); alternative way of dep. injection,
            // when constuctor injection is not applicable.
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointOfInterest(int cityId)
        {
            try
            {
               // throw new Exception("Oops");
               if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var pointsOfInterestForCityResults =
                    AutoMapper.Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);

                return Ok(pointsOfInterestForCityResults);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                return StatusCode(500, "An error occured while handling your requst.");
            }

        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterest == null)
                return NotFound();

            var pointOfInterestResult = AutoMapper.Mapper.Map<PointOfInterestDto>(pointOfInterest);
            return Ok(pointOfInterestResult);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("Description", "Fields 'name' and 'description' can't have equal values");

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var newPointOfInterest = AutoMapper.Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointsOfInterestForCity(cityId, newPointOfInterest);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem occurred while handling your request.");

            var createdPointOfInterestToReturn =
                AutoMapper.Mapper.Map<Models.PointOfInterestDto>(newPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new
            { cityId, id = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, 
            [FromBody] PointOfInterestUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("Description", "Fields 'name' and 'description' can't have equal values");

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            AutoMapper.Mapper.Map(pointOfInterest, pointOfInterestEntity);
            if (_cityInfoRepository.Save())
                return StatusCode(500, "A problem occurred while handling your request.");

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, 
            [FromBody] JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            var pointOfInterestToPatch = AutoMapper.Mapper.Map<PointOfInterestUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
                ModelState.AddModelError("Description", "Fields 'name' and 'description' can't have equal values");

            TryValidateModel(pointOfInterestToPatch);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AutoMapper.Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem occurred while handling your request.");

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem occurred while handling your request.");

            _mailService.Send("Point of interest deleted.", 
                $"Point of interest {pointOfInterestEntity.Name}" +
                $" with id {pointOfInterestEntity.Id} was deleted");

            return NoContent();
        }
    }
}
