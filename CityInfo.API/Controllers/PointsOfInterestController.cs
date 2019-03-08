using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {

        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        //bir şehire ait tüm poi'ler döner
        [HttpGet("{cityId}/pointsofinterest", Name = "GetPointOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
                try
                {
                //throw new Exception("Exception sample");

                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                //aranılan şehir yoksa 404 döndür.
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }
                //şehirin tüm poi'leri döner
                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

                //var pointsOfInterestForCityResults = new List<PointOfInterestDto>();
                //foreach (var poi in pointsOfInterestForCity)
                //{
                //    pointsOfInterestForCityResults.Add(new PointOfInterestDto()
                //    {
                //        Id = poi.Id,
                //        Name = poi.Name,
                //        Description = poi.Description
                //    });
                //}
                //yukarıdaki satırları maplemek için kapattık.

                var pointsOfInterestForCityResults = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);


                return Ok(pointsOfInterestForCityResults);

                //if (city == null)
                //{
                //    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                //    return NotFound();
                //}
                //return Ok(city.PointsOfInterest);

            }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                    return StatusCode(500, "A Problem happened while handling your request.");
                }

            }
        //tek bir poi döndürür
        [HttpGet("{cityId}/pointsofinterest/{id}")]
        public IActionResult GetPointsOfInterest(int cityId, int id)
        {
            //aranılan şehir yoksa 404 döndürür.
            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();
            //ilgili şehrin ilgili poi'si döner.
            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            //poi yok ise 404 döner.
            if (pointOfInterest == null)
                return NotFound();
            //poi'nin özellikleri bir nesneye yazıp döndürülür.
            //var pointOfInterestResult = new PointOfInterestDto()
            //{
            //    Id = pointOfInterest.Id,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};
            //Yukarıdaki satırları maplemek için kapattık

            var pointOfInterestResult = Mapper.Map<PointOfInterestDto>(pointOfInterest);

            return Ok(pointOfInterestResult);


            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //    return NotFound();

            //var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            //if (pointOfInterest == null)
            //    return NotFound();

            //return Ok(pointOfInterest);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {

            if (pointOfInterest == null)
                return BadRequest();



            if (pointOfInterest.Name == pointOfInterest.Description)
                ModelState.AddModelError("Description", "Description ile Name propertylerine aynı içerik atanamaz");



            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //    return NotFound();
            //yukarıdaki kapalı kodlar yerine aşağıdaki kod kullanılacak;

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            //var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
            //    c => c.PointsOfInterest).Max(p => p.Id);
            //yukarıdaki kapalı kodlara ihtiyaç yok

            //var finalPointOfInterest = new PointOfInterestDto()
            //{

            //    Id = ++maxPointOfInterestId,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description

            //};
            //yukarıdaki kapalı kodlar yerine aşağıdaki kod kullanılacak;

            var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while heading your request.");
            }

            //city.PointsOfInterest.Add(finalPointOfInterest);
            //bir üstteki kod bloğundan dolayı yukarıdaki kod satırı iptal edildi.

            var createdPointOfInterestToReturn = Mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);



            //201 Created Status Döndürür. Alttaki anonymous type içerisinde response body döndürülür. 
            //return CreatedAtRoute("GetPointOfInterest", new
            //{
            //    //Anonymous type
            //    cityId = cityId,
            //    id = finalPointOfInterest.Id,
            //    finalPointOfInterest
            //    //finalPointOfInteres.Name

            //});
            //Yukarıdaki kodlar aşağıdaki kodlar yüzünden! iptal edildi.

            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                id = createdPointOfInterestToReturn.Id,
            },
            createdPointOfInterestToReturn);


            //Headers/Location içerisinde de oluşan yeni pointOfInterest'in URI adresi görülür.

        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
                return BadRequest();

            if (pointOfInterest.Name == pointOfInterest.Description)
                ModelState.AddModelError("Description", "Name ve Description alanı aynı içeriğe sahip olamaz.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(p => p.Id == cityId);

            //if (city == null)
            //    return NotFound();
            //yukarıdaki kodlar yerine aşağıdaki kodlar kullanılıyor.

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();
            

            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            //if (pointOfInterestFromStore == null)
            //    return NotFound();
            //yukarıdaki kodlar yerine aşağıdaki kodlar çalışıyor.

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();
            
            //pointOfInterestFromStore.Name = pointOfInterest.Name;
            //pointOfInterestFromStore.Description = pointOfInterest.Description;
            //yukarıdaki kodlar yerine aşağıdaki kod çalışıyor.

            Mapper.Map(pointOfInterest, pointOfInterestEntity);

            //değişiklikleri kaydediyoruz.
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "a problem happened while handling your request.");
            }


            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {

            if (patchDoc == null)
                return BadRequest();

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(p => p.Id == cityId);

            //if (city == null)
            //    return NotFound();
            //yukarıdaki kodlar yerine aşağıdaki kodlar çalışır.

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            //if (pointOfInterestFromStore == null)
            //    return NotFound();
            //yukarıdaki kodlar yerine aşağıdaki kodlar çalışır.

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            //var pointOfInterestToPatch =
            //    new PointOfInterestForUpdateDto
            //    {
            //        Name = pointOfInterestFromStore.Name,
            //        Description = pointOfInterestFromStore.Description
            //    };

            var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity); 
           

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
                ModelState.AddModelError("Description", "Name ve description alanları aynı değeri alamaz");

            TryValidateModel(pointOfInterestToPatch);   //bozulmus mu kontrolü


            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            //pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            //pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
            //Yukarıdaki kod yerine aşağıdaki kod parçası kullanılıyor. (fromstore alanının adı entity olarak değişti)

            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity); //soldan sağa veri aktarımı mevcut.
            //passing in the source object, which is the Patch Dto, and the destination object,
            //which is the entity.

            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }


            return NoContent();
        }


        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(p => p.Id == cityId);

            //if (city == null)
            //    return NotFound();
            //yukarıdaki kodlar yerine aşağıdaki kodlar kullanılır.

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            //if (pointOfInterestFromStore == null)
            //    return NotFound();
            //yukarıdaki kodlar yerine aşağıdaki kodlar kullanılır

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            //city.PointsOfInterest.Remove(pointOfInterestFromStore);
            //yukarıdaki kod yerin aşağıdaki kod kullanılır.

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem happened while handling you request");

            //_mailService.Send("Point Of Interest deleted.", 
            //    $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

            _mailService.Send("Point Of Interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");


            return NoContent();

        }


    }
}
