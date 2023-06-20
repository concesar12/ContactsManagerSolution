using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
    public class PersonAlwaysRunResultFilter :IAlwaysRunResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            //Validate if the skip filter apply
            if (context.Filters.OfType<SkipFilter>().Any())
            {
                return;
            }

            //TO DO: before logic here
        }
    }
}