using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Linq;

namespace ProjectManagment.Tests
{
    [TestFixture]
    public class CreateProject
    {
        private EmbeddableDocumentStore _embeddedDocStore;
        
        [SetUp]
        public void SetupTests()
        {
            _embeddedDocStore = new EmbeddableDocumentStore { RunInMemory = true };
            _embeddedDocStore.Initialize();
            var documentSession = _embeddedDocStore.OpenSession();
            documentSession.Store(new Project { Name = "existing" });
            documentSession.Store(new UserAccount { Username = "inactive", Status = UserStatus.Inactive });
            documentSession.SaveChanges();
        }

        [Test]
        public void CreateNewProjectUniqueName()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            const string projectName = "newProject";
            var user = new UserAccount();
            var createdProject = pm.CreateProject(projectName, user);
            Assert.That(createdProject.Owner, Is.EqualTo(user.Username));
            Assert.That(createdProject.Name, Is.EqualTo(projectName));
            AssertProjectIsCreated(projectName, user.Username);
        }

        private void AssertProjectIsCreated(string projectName, string username)
        {
            var session = _embeddedDocStore.OpenSession();
            var projectCount = session.Query<Project>().Where(p => p.Name == projectName && p.Owner == username).Count();
            Assert.That(projectCount, Is.EqualTo(1));
        }

        [Test]
        public void CreateNewProjectDuplicateName()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            const string projectName = "existing";
            var user = new UserAccount();
            var exceptionMessage = string.Format("Cannot create new project '{0}' - this project name is already in use.", projectName);
            Assert.That(() => pm.CreateProject(projectName, user), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void CreateNewProjectInactiveUser()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            var user = new UserAccount { Username = "inactive" };
            const string projectName = "project";
            var exceptionMessage = string.Format("Cannot create new project '{0}' - {1} is an inactive user.", projectName, user.Username);
            Assert.That(() => pm.CreateProject(projectName, user), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }
    }

    public class UserAccount
    {
        public string Username { get; set; }

        public UserStatus Status { get; set; }
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }

    public interface IProjectManager
    {
        Project CreateProject(string projectName, UserAccount user);
    }

    public class Project
    {
        public string Owner { get; set; }

        public string Name { get; set; }
    }

    public class ProjectManager : IProjectManager
    {
        public IDocumentStore DocumentStore { get; set; }

        public Project CreateProject(string projectName, UserAccount user)
        {
            var session = DocumentStore.OpenSession();
            var inactiveUserAccount = session.Query<UserAccount>().Where(u => u.Username == user.Username && u.Status == UserStatus.Inactive).Count();
            if (inactiveUserAccount > 0)
                throw new ArgumentException(string.Format("Cannot create new project '{0}' - {1} is an inactive user.", projectName, user.Username));
            var existingProjects = session.Query<Project>().Where(p => p.Name == projectName).Count();
            if (existingProjects > 0)
                throw new ArgumentException(string.Format("Cannot create new project '{0}' - this project name is already in use.", projectName));
            var project = new Project { Name = projectName, Owner = user.Username };
            session.Store(project);
            session.SaveChanges();
            return project;
        }
    }
}
