using System.Web.Mvc;

namespace ProjectManagement.Web.Controllers
{
    public class HomeController : RavenController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
