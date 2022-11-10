﻿using System;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
	public interface ICityInfoRepository
	{
		//Could use the IQueryable<City> approach as well
		//Using IQueryable allows you to build on the query before it is even executed EX: ORDER BY
		//By using Task, this makes the method asynchronous
		Task<IEnumerable<City>> GetCitiesAsync();

		Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);

		Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId);

		Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);

	}
}
