using System;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
	public interface ICityInfoRepository
	{
		//Could use the IQueryable<City> approach as well
		//Using IQueryable allows you to build on the query before it is even executed EX: ORDER BY
		//By using Task, this makes the method asynchronous
		Task<IEnumerable<City>> GetCitiesAsync();
		Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

		Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);

		Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId);

		Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);

		Task<bool> CityExistsAsync(int cityId);

		Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

		void DeletePointOfInterest(PointOfInterest pointOfInterest);
		Task<bool> CityNameMatchesCityId(string? cityName, int cityId);

		Task<bool> SaveChangesAsync();
    }
}

