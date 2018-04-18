using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(ct => ct.Id == cityId);
            if (city == null) { return NotFound(); }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(ct => ct.Id == cityId);
            if (city == null) { return NotFound(); }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(pt => pt.Id == id);
            if (pointOfInterest == null) { return NotFound(); }

            return Ok(pointOfInterest);
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

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(ct => ct.Id == cityId);
            if (city == null) { return NotFound(); }

            var lastPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                ct => ct.PointsOfInterest).Max(m => m.Id);

            var newPointOfInterest = new PointOfInterestDto()
            {
                Id = ++lastPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            return CreatedAtRoute("GetPointOfInterest", new
            { cityId, id = newPointOfInterest.Id }, newPointOfInterest);
        }
    }
}
