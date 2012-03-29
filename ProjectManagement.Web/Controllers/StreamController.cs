using System;
using System.Linq;
using System.Web.Mvc;
using Raven.Client.Linq;

namespace ProjectManagement.Web.Controllers
{
    public class StreamController : RavenController
    {
        //
        // GET: /Stream/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Stream/Details/5

        public PartialViewResult Details(string projectName, string streamName)
        {
            var stream = RavenSession.Query<Project>().Where(p => p.Name == projectName).First().ProjectStreams.Where(s => s.Name == streamName).First();
            return PartialView("_StreamDetails", stream);
        }

        //
        // GET: /Stream/Create

        public PartialViewResult Create(string projectName)
        {
            ViewBag.ProjectName = projectName;
            return PartialView("_StreamCreate");
        }

        [HttpPost]
        public PartialViewResult Create(FormCollection collection)
        {
            try
            {
                var streamName = collection["name"];
                var description = collection["description"];
                var projectName = collection["projectName"];
                var projectStream = new ProjectStream(streamName, description);

                var projectManager = new ProjectManager { Session = RavenSession };
                projectManager.AddProjectStream(projectName, projectStream, User.Identity.Name);
                return Details(projectName, streamName);
                // return RedirectToAction("Details", new { projectName = projectName, streamName = projectStream.Name});
            }
            catch(Exception e)
            {
                throw;
            }
        }
        
        //
        // GET: /Stream/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Stream/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Stream/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Stream/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
