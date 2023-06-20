using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters
{
    //When inheriting from attribute then this class can be part of a controller or action method
    public class SkipFilter :Attribute, IFilterMetadata
    {
    }
}