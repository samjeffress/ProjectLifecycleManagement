using System.Linq;
using System.Web.Mvc;
using ProjectManagement.Web.Helpers;
using Raven.Client.Linq;

namespace ProjectManagement.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : RavenController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            var activeProjects = RavenSession.Query<Project>().Where(p => p.Status == Status.Active).ToList();

            return View("Index", activeProjects);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }
    }
}
