using AutoMapper;

namespace CityInfo.API.Profiles
{
	public class CityProfile : Profile
	{
		public CityProfile()
		{
			//source - destination
			CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDTO>();

			CreateMap<Entities.City, Models.CityDto>();
		}
	}
}

