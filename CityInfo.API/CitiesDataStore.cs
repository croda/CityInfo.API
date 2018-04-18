using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        // ensure we keep working on the same data until we restart
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            // dummy data
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "Skopje",
                    Description = "Capitol of Macedonia",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "City Park",
                            Description = "Biggest urban park in the city"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "City Square",
                            Description = "City centre busting with life"
                        }
                    }
                },

                new CityDto()
                {
                    Id = 2,
                    Name = "Zagreb",
                    Description = "Capitol of Croatia",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Zagreb Cathedral",
                            Description = "Most visited attraction in Zagreb"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Jarun Lake",
                            Description = "Primary spot for rowing, sailing, swimming, etc"
                        }
                    }
                }
            };
        }
    }
}
