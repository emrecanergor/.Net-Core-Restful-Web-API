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

            if (context.Cities.Any())   //cities is dbset
                return;

            var cities = new List<City>()
            {

                new City()
                {
                    Name = "New York City",
                    Description = "Big Park",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "Central Park",
                            Description = "United States"
                        },
                        new PointOfInterest()
                        {
                            Name = "Empire States Building",
                            Description = "Midtown Manhattan"
                        }
                    }

                },
                new City()
                {
                    Name = "Paris",
                    Description = "Big Tower",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "Eiffel Tower",
                            Description = "Champ de Mars"
                        },
                        new PointOfInterest()
                        {
                            Name = "The Louvre",
                            Description = "The Largest Museum"
                        }
                    }
                }

            };

            context.Cities.AddRange(cities);
            context.SaveChanges();

        }


    }
}
