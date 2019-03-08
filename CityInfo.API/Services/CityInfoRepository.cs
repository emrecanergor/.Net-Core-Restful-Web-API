using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {

        private CityInfoContext _context;
        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool CityExists(int cityId)
        {
            return _context.Cities.Any(c => c.id == cityId);
        }

        //pointsofinterest kısmı gelmez, include etmeliyiz.
        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        //pointsofinterest kısmı için include kullanıyoruz.
        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            //eğer poi isteniyorsa include ile gönderilir.
            if (includePointsOfInterest)
            {
                return _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.id == cityId).FirstOrDefault();
            }
            //istenmiyorsa null döndürülür.
            return _context.Cities.Where(c => c.id == cityId).FirstOrDefault();
            
        }

        //belirli bir city'nin belirli bir pointofinterest'ini döndürür.
        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _context.PointsOfInterest
                .Where(c => c.CityId == cityId && c.Id == pointOfInterestId).FirstOrDefault();
        }

        //belirli bir city'nin tüm pointofinterest'ini döndürür.
        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _context.PointsOfInterest
                .Where(c => c.CityId == cityId).ToList();
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }
    }
}
