using CityInfo.API.Models;
using CityInfo.API.Services;
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
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("/api/cities")]   // /api/cities is the routing template
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();
            var results = AutoMapper.Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
 
            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (city == null)
                return NotFound();

            if (includePointsOfInterest)
            {
                var cityResult = AutoMapper.Mapper.Map<CityDto>(city);
                return Ok(cityResult);
            }

            var CityWitoutPointsOfInterestResult = AutoMapper.Mapper.Map<CityWithoutPointsOfInterestDto>(city);
            return Ok(CityWitoutPointsOfInterestResult);
        }
    }
}
