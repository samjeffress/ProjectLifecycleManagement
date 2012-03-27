using System.Web.Mvc;
using ProjectManagement.Web.Controllers;

namespace ProjectManagement.Web.Helpers
{
    // From http://blogs.msdn.com/b/rickandy/archive/2011/05/02/securing-your-asp-net-mvc-3-application.aspx
    public class LogonAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!(filterContext.Controller is AccountController))
                base.OnAuthorization(filterContext);
        }
    }
}