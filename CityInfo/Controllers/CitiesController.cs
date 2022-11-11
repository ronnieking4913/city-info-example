using System;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        //private readonly CitiesDataStore _citiesDataStore;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            //_citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));  //null check on city datastore
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDTO>>> GetCities(
            // Filtering the cities based on the name to be filtered
            string? name,
            // allowing the api to do searches
            string? searchQuery)
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync(name, searchQuery);

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDTO>>(cityEntities));

            //return Ok(_citiesDataStore.Cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCiy(int id, bool includePointsOfInterest = false)
        {

            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

            if(city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDTO>(city));
            //var cityFound = _citiesDataStore.Cities
            //    .FirstOrDeFault();

            //if (cityFound == null)
            //{
            //    return NotFound();
            //}

            //return Ok(cityFound);
        }
    }
}

