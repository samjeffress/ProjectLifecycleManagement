using System;
using System.Web.Mvc;

namespace ProjectManagement.Web.Controllers
{
    public class ProjectController : RavenController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            var project = RavenSession.Load<Project>(id);
            if (project == null)
                throw new ArgumentException("Can't find project " + id);
            return View("Details", project);
        }

        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                var project = new Project();
                UpdateModel(project);
                var pm = new ProjectManager { Session = RavenSession };
                var createdProject = pm.CreateProject(project.Name, User.Identity.Name);

                return RedirectToAction("Details", new { id = project.Name });
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Project/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Project/Edit/5

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
        // GET: /Project/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Project/Delete/5

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
