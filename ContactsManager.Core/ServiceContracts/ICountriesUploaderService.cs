using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesUploaderService
    {
        //Task UploadCountriesFromExcelFile(IEnumerable<IFormFile> formFile); In case wanting to have the chance to upload 1+ file

        /// <summary>
        /// Uploads countries from excel file into database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>Returns number of countries</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}