using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    //[Route("apo/[controller]")] matches /api/cities for all methods/actions in this class
    [Route("api/cities")] // cities should remain the same regardless of the name of the class
    public class CitiesController : Controller
    {
        [HttpGet("/api/cities")]   // /api/cities is the routing template
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(ct => ct.Id == id);
            if (cityToReturn == null) { return NotFound(); }

            return Ok(cityToReturn);
        }
    }
}
