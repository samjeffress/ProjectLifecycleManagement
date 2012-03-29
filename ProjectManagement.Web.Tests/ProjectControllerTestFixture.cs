using System;
using System.Web.Mvc;
using NUnit.Framework;
using ProjectManagement.Web.Controllers;

namespace ProjectManagement.Web.Tests
{
    [TestFixture]
    public class ProjectControllerTestFixture
    {
        //[Test]
        //public void CreateProjectProjectManagerThrowsExceptionReturnsToCreateWithError()
        //{
        //    var projectManager = MockRepository.GenerateMock<IProjectManager>();
        //    var exception = new Exception("Something bad happened");
        //    projectManager.Expect(p => p.CreateProject("Name", username)).Throw(exception);

        //    // TODO: Mock Http context - specifically the user
        //    var controller = new ProjectController { ProjectManager = projectManager };
        //    var collection = new FormCollection {{"Name", "AlreadyExists"}};
        //    var actionResult = controller.Create(collection) as ViewResult;

        //    Assert.That(actionResult.ViewName, Is.EqualTo("Create"));

        //    projectManager.VerifyAllThings();
        //}
        
        //[Test]
        //public void CreateProjectSuccessReturnsToDetailsPage()
        //{
        //    // TODO: Mock Http context - specifically the user
        //    const string username = "Boo";
        //    var newProject = new Project("projectName", username);

        //    var projectManager = MockRepository.GenerateMock<IProjectManager>();            
        //    projectManager.Expect(p => p.CreateProject("Name", username)).Return(newProject);

        //    var controller = new ProjectController { ProjectManager = projectManager };
        //    var collection = new FormCollection {{"Name", "AlreadyExists"}};
        //    var actionResult = controller.Create(collection) as RedirectToRouteResult;

        //    Assert.That(actionResult.RouteName, Is.EqualTo("Details"));
        //    Assert.That(actionResult.RouteValues, Is.EqualTo(new { id = newProject.Name }));}

        //    projectManager.VerifyAllThings();
        //}
    }
}
