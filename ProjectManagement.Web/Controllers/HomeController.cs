using System.Linq;
using System.Web.Mvc;
using Raven.Client.Linq;

namespace ProjectManagement.Web.Controllers
{
    public class HomeController : RavenController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            var activeProjects = RavenSession.Query<Project>().Where(p => p.Status == Status.Active).ToList();

            return View("Index", activeProjects);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
