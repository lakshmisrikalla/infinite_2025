using System.Web;
using System.Web.Mvc;

namespace Web_Api_Code_Challenge_10
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
