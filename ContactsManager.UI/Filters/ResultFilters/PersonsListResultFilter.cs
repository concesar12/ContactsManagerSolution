using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
    public class PersonsListResultFilter :IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilter> _logger;

        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //TO DO: before logic
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
            //This will be added as a reponse header
            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            //General rule is that, you cannot add a response header after starting of streaming (start sending) response to the client.
            await next(); //call the subsequent filter [or] IActionResult

            //TO DO: after logic
            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
        }
    }
}