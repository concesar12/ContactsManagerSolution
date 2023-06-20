using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.AuthorizationFilter
{
    public class TokenAuthorizationFilter :IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //Here we will verify if the cookie has auth cookie
            if (context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") == false)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }
            //We have to check if the cookie complies as per the requirement
            if (context.HttpContext.Request.Cookies["Auth-Key"] != "A100")
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
        }
    }
}