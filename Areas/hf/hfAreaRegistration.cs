using System.Web.Mvc;

namespace Informer.Areas.hf
{
    public class hfAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "hf";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "hf_default",
                "hf/{controller}/{action}/{id}",
                new { areas = "hf", controller = "Item", action = "Detail", id = UrlParameter.Optional }
                );
        }
    }
}
