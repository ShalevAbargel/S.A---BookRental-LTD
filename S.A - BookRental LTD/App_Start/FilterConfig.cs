using System.Web;
using System.Web.Mvc;

namespace S.A___BookRental_LTD
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
