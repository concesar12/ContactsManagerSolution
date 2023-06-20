using CRUDExample.Filters;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilter;
using CRUDExample.Filters.ExceptionFilters;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    //[Route("persons")] // This is done to avoid writing persons for evey route
    [Route("[controller]")] // This takes the action method controller name that is persons
                            //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-Key-From-Controller", "My-Value-From-Controller", 3 }, Order = 3)]
    [ResponseHeaderFilterFactory("My-Key-From-Controller", "My-Value-From-Controller", 3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysRunResultFilter))]
    public class PersonsController :Controller
    {
        //private fields from our services
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;

        private readonly ICountriesGetterService _countriesService;


        //Create the log
        private readonly ILogger<PersonsController> _logger;

        //Constructor
        public PersonsController(IPersonsGetterService personsGetterService,
            ICountriesGetterService countriesService,
            ILogger<PersonsController> logger,
            IPersonsAdderService personsAdderService,
            IPersonsDeleterService personsDeleterService,
            IPersonsSorterService personsSorterService,
            IPersonsUpdaterService personsUpdaterService)
        {
            _personsGetterService = personsGetterService;
            _countriesService = countriesService;
            _logger = logger;
            _personsAdderService = personsAdderService;
            _personsDeleterService = personsDeleterService;
            _personsSorterService = personsSorterService;
            _personsUpdaterService = personsUpdaterService;
        }

        //URL: persons/index
        [Route("[action]")] // Represents the name of the action below
        [Route("/")]
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)] //Filter without arguments
        //Commented because of functionality of ResponseHeaderActionFilter
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "MyKey-FromAction", "MyValue-From-Action", 1 }, Order = 1)] //Filter with arguments
        [ResponseHeaderFilterFactory("MyKey-FromAction", "MyValue-From-Action", 1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //Log information
            _logger.LogInformation("Index action method of PersonsController");
            //Log Debug
            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

            //Search
            List<PersonResponse> persons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);
            //Not necessary anymore because we added in personsListFilter
            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);

            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        //Executes when the user clicks on "Create Person" hyperlink (while opening the create view)
        [Route("[action]")]
        [HttpGet] // This means that that method only receive get requests
        [ResponseHeaderFilterFactory("my-key", "my-value", 4)]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
              new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() }
            );

            //new SelectListItem() { Text="Harsha", Value="1" }
            //<option value="1">Harsha</option>
            return View();
        }

        //Url: persons/Create
        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            /*This was passed to the filter to make it more generic*/
            //if (!ModelState.IsValid)
            //{
            //    List<CountryResponse> countries = await _countriesService.GetAllCountries();
            //    ViewBag.Countries = countries;

            //    ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            //    return View(personRequest);
            //}

            //call the service method
            PersonResponse personResponse = await _personsAdderService.AddPerson(personRequest);

            //navigate to Index() action method (it makes another get request to "persons/index"
            return RedirectToAction("Index", "Persons");
        }

        //Executes when the user clicks on "Create Person" hyperlink (while opening the create view)
        //Url: persons/create
        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
            PersonResponse updatedPerson = await _personsUpdaterService.UpdatePerson(personRequest);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personUpdateResult.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            await _personsDeleterService.DeletePerson(personUpdateResult.PersonID);
            return RedirectToAction("Index");
        }

        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //Get list of persons
            List<PersonResponse> persons = await _personsGetterService.GetAllPersons();

            //Return view as pdf
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}