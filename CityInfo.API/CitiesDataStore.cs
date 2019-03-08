using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{

    public class CitiesDataStore
    {
        //Dummy Data'yı tutan liste
        public List<CityDto> Cities { get; set; }

        //statik nesne
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public CitiesDataStore() {

            Cities = new List<CityDto>
         {
            new CityDto()
            {
                Id = 1,
                Name = "NYC",
                Description = "Big park",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Central Park",
                        Description = "Muazzam bir park"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "Merkez Park",
                        Description = "Camii'si muazzam"
                    }

                }
            },

            new CityDto()
            {
                Id = 2,
                Name = "Antwerp",
                Description = "Cathedral",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Yıldız Park",
                        Description = "Buralarda bir yerde"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "Gülhane Parkı",
                        Description = "Ceviz Ağaçları çok güzel"
                    }
                }

            },

            new CityDto()
            {
                Id = 3,
                Name = "Paris",
                Description = "Big Tower",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Mahalle Parkı",
                        Description = "Çocukların Şen Yuvası"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "Cumhuriyet Parkı",
                        Description = "Havası Çok güzel"
                    }
                }
            }
         };
        }

    }
}
