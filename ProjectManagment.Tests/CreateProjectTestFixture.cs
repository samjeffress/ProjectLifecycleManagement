using System.Linq;
using NUnit.Framework;
using ProjectManagement;
using Raven.Client.Embedded;
using Raven.Client.Linq;

namespace ProjectManagment.Tests
{
    [TestFixture]
    public class CreateProjectTestFixture
    {
        private EmbeddableDocumentStore _embeddedDocStore;
        
        [TestFixtureSetUp]
        public void SetupTests()
        {
            _embeddedDocStore = new EmbeddableDocumentStore { RunInMemory = true };
            _embeddedDocStore.Initialize();
            var documentSession = _embeddedDocStore.OpenSession();
            documentSession.Store(new Project("existing", ""));
            documentSession.Store(new UserAccount { Username = "inactive", Status = UserStatus.Inactive });
            documentSession.Store(new UserAccount { Username = "user", Status = UserStatus.Active});
            documentSession.SaveChanges();
        }

        [Test]
        public void CreateNewProjectUniqueName()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            const string projectName = "newProject";
            const string user = "user";
            var createdProject = pm.CreateProject(projectName, user);
            Assert.That(createdProject.Owner, Is.EqualTo(user));
            Assert.That(createdProject.Name, Is.EqualTo(projectName));
            AssertProjectIsCreated(projectName, user);
        }

        [Test]
        public void CreateNewProjectDuplicateName()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            const string projectName = "existing";
            const string user = "user";
            var exceptionMessage = string.Format("Cannot create new project '{0}' - this project name is already in use.", projectName);
            Assert.That(() => pm.CreateProject(projectName, user), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void CreateNewProjectInactiveUser()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            const string user = "inactive";
            const string projectName = "project";
            var exceptionMessage = string.Format("Cannot create new project '{0}' - {1} is an inactive user.", projectName, user);
            Assert.That(() => pm.CreateProject(projectName, user), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        private void AssertProjectIsCreated(string projectName, string username)
        {
            var session = _embeddedDocStore.OpenSession();
            var projectCount = session.Query<Project>().Where(p => p.Name == projectName && p.Owner == username).Count();
            Assert.That(projectCount, Is.EqualTo(1));
            var project = session.Query<Project>().Where(p => p.Name == projectName && p.Owner == username).First();
            Assert.That(project.Users.Count(), Is.EqualTo(1));
            Assert.That(project.Users[0], Is.EqualTo(username));
        }
    }
}
