using System.Web;
using System.Web.Mvc;

namespace ViViPOSLive_Sample_AspNet
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
