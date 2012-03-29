using System;
using System.Web.Mvc;

namespace ProjectManagement.Web.Controllers
{
    public class ProjectController : RavenController
    {
        public IProjectManager ProjectManager { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            try
            {
                var project = RavenSession.Load<Project>(id);
                if (project == null)
                    throw new ArgumentException("Cannot find project " + id);
                return View("Details", project);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Error");
            }
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
                ProjectManager.CreateProject(project.Name, User.Identity.Name);
                return RedirectToAction("Details", new { id = project.Name });
            }
            catch(Exception e)
            {
                
                // TODO: Push exception in here rather than redirect to an error page
                return View("Create");
            }
        }
 
        public ActionResult Edit(string id)
        {
            try
            {
                var project = RavenSession.Load<Project>(id);
                return View("Edit", project);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Error");
            }
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
