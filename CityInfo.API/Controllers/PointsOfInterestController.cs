﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{cityId}/pointsofinterest/{id}")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(ct => ct.Id == cityId);
            if (city == null) { return NotFound(); }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(pt => pt.Id == id);
            if (pointOfInterest == null) { return NotFound(); }

            return Ok(pointOfInterest);
        }
    }
}