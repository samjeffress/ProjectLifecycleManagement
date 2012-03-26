using System.Linq;
using NUnit.Framework;
using ProjectManagement;
using Raven.Client.Embedded;
using Raven.Client.Linq;

namespace ProjectManagment.Tests
{
    [TestFixture]
    public class CreateProjectStream
    {
        private EmbeddableDocumentStore _embeddedDocStore;


        private static UserAccount activeUserNotInProject = new UserAccount { Username = "activeUserNotInProject", Status = UserStatus.Active };
        private static UserAccount activeUser = new UserAccount { Username = "activeUser", Status = UserStatus.Active };
        private static Project activeProject = new Project("existing", activeUser.Username) { Status = ProjectStatus.Active };
        private static Project cancelledProject = new Project("cancelled", activeUser.Username) { Status = ProjectStatus.Cancelled };
        private static Project completedProject = new Project("completed", activeUser.Username) { Status = ProjectStatus.Complete };

        [SetUp]
        public void SetupTests()
        {
            _embeddedDocStore = new EmbeddableDocumentStore { RunInMemory = true };
            _embeddedDocStore.Initialize();
            var documentSession = _embeddedDocStore.OpenSession();
            documentSession.Store(activeUser);
            documentSession.Store(activeUserNotInProject);
            documentSession.Store(activeProject);
            documentSession.Store(cancelledProject);
            documentSession.Store(completedProject);
            documentSession.SaveChanges();
        }

        [Test]
        public void AddStreamToActiveProjectSuccess()
        {
            var projectManager = new ProjectManager { DocumentStore = _embeddedDocStore };
            var newProjectStream = new ProjectStream("Stream1", "Create a new stream that takes over the whole project.");
            projectManager.AddProjectStream(activeProject, newProjectStream, activeUser.Username);
            var session = _embeddedDocStore.OpenSession();
            var project = session.Query<Project>().Where(p => p.Name == activeProject.Name).First();
            Assert.That(project.ProjectStreams.Count, Is.EqualTo(1));
            Assert.That(project.ProjectStreams[0].CreatedBy, Is.EqualTo(activeUser.Username));
        }

        [Test]
        public void AddStreamToActiveProjectUserNotInProjectThrowsException()
        {
            var projectManager = new ProjectManager { DocumentStore = _embeddedDocStore };
            var newProjectStream = new ProjectStream("Stream1", "Create a new stream that takes over the whole project.");
            var exceptionMessage = string.Format("User {0} is currently not active in Project {1}.", activeUserNotInProject.Username, activeProject.Name);
            Assert.That(() => projectManager.AddProjectStream(activeProject, newProjectStream, activeUserNotInProject.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void AddStreamToCancelledProjectThrowsException()
        {
            var projectManager = new ProjectManager { DocumentStore = _embeddedDocStore };
            var newProjectStream = new ProjectStream("Stream1", "Create a new stream that takes over the whole project.");
            const string exceptionMessage = "Cannot add stream to project with status of Cancelled.";
            Assert.That(() => projectManager.AddProjectStream(cancelledProject, newProjectStream, activeUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void AddStreamToCompleteProjectThrowsException()
        {
            var projectManager = new ProjectManager { DocumentStore = _embeddedDocStore };
            var newProjectStream = new ProjectStream("Stream1", "Create a new stream that takes over the whole project.");
            const string exceptionMessage = "Cannot add stream to project with status of Complete.";
            Assert.That(() => projectManager.AddProjectStream(completedProject, newProjectStream, activeUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }
    }
}
