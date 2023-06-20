using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute :Attribute, IFilterFactory
    {
        //Means can be accesible through multiple requests
        public bool IsReusable => false;

        private string? Key { get; set; }
        private string? Value { get; set; }
        private int Order { get; set; }

        //Contructor
        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            Key = key;
            Value = value;
            Order = order;
        }

        //Controller -> FilterFactory -> Filter
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            //Service provider help us inject methods
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;
            //return filter object
            return filter;
        }
    }

    public class ResponseHeaderActionFilter :IAsyncActionFilter, IOrderedFilter
    {
        //Creating logger
        private readonly ILogger<ResponseHeaderActionFilter> _logger; // We can't inject logger because ActionFilterAttribute does not allow

        //Response header key
        public string Key { get; set; }

        //Response header value
        public string Value { get; set; }

        //This comes with IOrderedFilter after implemet interface
        public int Order { get; set; } //This is precreated by ActionFilterAttribute

        //Constructor initialize variables
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        /*This piece is not longer necessary as we are using an async filter*/
        ////before
        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    _logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));
        //}

        ////after
        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    _logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));
        //    //Contexts will allow to access to the request, response and session object
        //    context.HttpContext.Response.Headers[_key] = _value;
        //}

        //Now we can use async in here //This will handle both previous
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("Before logic - ResponseHeaderActionFilter");

            await next(); //calls the subsequent filter or action method It is necessary to go to after

            _logger.LogInformation("Before logic - ResponseHeaderActionFilter");

            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}