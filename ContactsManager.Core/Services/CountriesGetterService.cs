using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesGetterService :ICountriesGetterService
    {
        //private field
        private readonly ICountriesRepository _countriesRepository;

        //constructor
        public CountriesGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        //Convert all countries from "Country" type to "CountryResponse" type
        //Return all CountryResponse objects
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country> countries = await _countriesRepository.GetAllCountries();
            return countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            //Check if "countryID" is not null
            //Get matching country from List<Country> based countryID
            //Convert matching country object from "Country" to "CountryResponse" type.
            //Return Country Response object
            if (countryID == null) return null;

            Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);

            if (country_response_from_list == null) return null;

            return country_response_from_list.ToCountryResponse();
        }

    }
}