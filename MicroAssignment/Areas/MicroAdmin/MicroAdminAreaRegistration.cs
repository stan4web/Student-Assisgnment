using System.Web.Mvc;

namespace MicroAssignment.Areas.MicroAdmin
{
    public class MicroAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MicroAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MicroAdmin_default",
                "MicroAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
