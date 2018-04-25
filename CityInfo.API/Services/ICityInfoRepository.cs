using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        bool CityExists(int cityId);
        IEnumerable<City> GetCities(); // can also be IQueryable (leaking persistence logic out of repository).
        City GetCity(int cityId, bool includePointsOfInterest);
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);
        IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId);
        void AddPointsOfInterestForCity(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
        bool Save();
    }
}
