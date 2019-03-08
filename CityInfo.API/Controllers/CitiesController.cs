using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;
        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }


        /// <summary>
        /// Retrieves the list of values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCities()
        {
            //return Ok(CitiesDataStore.Current.Cities);

            //bu değişkene aslında zaten pointofinterest'ler gelmeyecek. (null dönecek.)
            var cityEntities = _cityInfoRepository.GetCities();

            //var results = new List<CityWithoutPointsOfInterestDto>();

            //null olan bölümü boşa bulundurmamak için yeni bir Dto ile yeni bir liste oluşturuyoruz.
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto
            //    {
            //        Id = cityEntity.id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name
            //    });
            //}
            //Yukarıdaki foreach yerine şu satırları mapper sayesinde kullanabiliriz:

            //her bir item için mapleme işlemi aşağıdaki satırı kullanarak yapılır.
            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

            return Ok(results);
        }


        [HttpGet("{id}")]
        public IActionResult GetCities(int id, bool includePointsOfInterest = false)
        {
            //belirli şehir döndürülür -parametre durumuna göre poi dolu veya null
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (city == null)
                return NotFound();

            //poi dahil ise önce city özellikleri doldurulur.
            if (includePointsOfInterest)
            {
                //var cityResult = new CityDto()
                //{
                //    Id = city.id,
                //    Name = city.Name,
                //    Description = city.Description
                //};

                ////city özelliklerine eklenecek pointsOfInterest nesnesi doldurulur
                //foreach (var poi in city.PointsOfInterest)
                //{
                //    cityResult.PointsOfInterest.Add(
                //        new PointOfInterestDto()
                //        {
                //            Id = poi.Id,
                //            Name = poi.Name,
                //            Description = poi.Description
                //        });
                //}
                //Yukarıdaki satırları maplemek için kapattık - aynı işi yapıyorlar.

                var cityResult = Mapper.Map<CityDto>(city);

                return Ok(cityResult);
            }
            //else, poi'siz Dto kullanılır ve şehir döndürülür.
            //var cityWithoutPointsOfInterestResult =
            //    new CityWithoutPointsOfInterestDto()
            //    {
            //        Id = city.id,
            //        Name = city.Name,
            //        Description = city.Description
            //    };
            //Yukarıdaki kodları maplemek için kapattık

            var cityWithoutPointsOfInterestResult = Mapper.Map<CityWithoutPointsOfInterestDto>(city);

            return Ok(cityWithoutPointsOfInterestResult);



            //find city without bool parameter
            //var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            //if (cityToReturn == null)
            //{
            //    return NotFound();
            //}
            //return Ok(cityToReturn);

        }

        //----------------------------------------------------------------------------

        //[HttpGet]
        //public JsonResult GetCities()
        //{
        //    return new JsonResult(CitiesDataStore.Current.Cities);

        //}


        //[HttpGet("{id}")]
        //public JsonResult GetCities(int id)
        //{
        //    return new JsonResult(
        //        CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id)

        //        );

        //}

        //--------------------------------------------------------------------------

        //[HttpGet("api/cities")]
        //public JsonResult GetCities()
        //{
        //    return new JsonResult(new List<object>()
        //    {
        //        new { id=1, Name="Adana"},
        //        new { id=2, Name="Adıyaman"}
        //    });

        //}

        //[HttpPost("api/cities")]
        //public JsonResult PostCities([FromBody] int a)
        //{
        //    if (a.ToString().Length > 10)
        //        return new JsonResult("sa");

        //    return new JsonResult(new
        //    {

        //        id = 5,
        //        Name = "adsa",

        //    });

        //}


    }
}
