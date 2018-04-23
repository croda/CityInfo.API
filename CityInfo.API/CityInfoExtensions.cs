using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public static class CityInfoExtensions
    {
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            if (context.Cities.Any())
                return;

            var cities = new List<City>()
            {
                new City()
                {
                    Name = "Skopje",
                    Description = "Capitol of Macedonia",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "City Park",
                            Description = "Biggest urban park in the city"
                        },
                        new PointOfInterest()
                        {
                            Name = "City Square",
                            Description = "City centre busting with life"
                        }
                    }
                },

                new City()
                {
                    Name = "Zagreb",
                    Description = "Capitol of Croatia",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "Zagreb Cathedral",
                            Description = "Most visited attraction in Zagreb"
                        },
                        new PointOfInterest()
                        {
                            Name = "Jarun Lake",
                            Description = "Primary spot for rowing, sailing, swimming, etc"
                        }
                    }
                }
            };

            context.Cities.AddRange(cities);
            context.SaveChanges(); // execute statements on our DB
        }
    }
}
