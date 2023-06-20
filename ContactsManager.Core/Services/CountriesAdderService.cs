using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesAdderService :ICountriesAdderService
    {
        //private field
        private readonly ICountriesRepository _countriesRepository;

        //constructor
        public CountriesAdderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddRequest parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: countryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            //TODO Validate the name with a Regex to check Capital letters-numbers or special characters
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException($"Given country name {countryAddRequest.CountryName} already exists");
            }
            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //Generate a new GUID countryID
            country.CountryID = Guid.NewGuid();

            //Add country object into _countries
            await _countriesRepository.AddCountry(country);
            return country.ToCountryResponse();
        }

    }
}