using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectManagement;
using Raven.Client.Embedded;
using Raven.Client.Linq;

namespace ProjectManagment.Tests
{
    [TestFixture]
    public class CreateProjectGoalsTestFixture
    {
        private EmbeddableDocumentStore _embeddedDocStore;

        readonly UserAccount _existingUser = new UserAccount { Username = "existingUser", Status = UserStatus.Active };
        readonly UserAccount _existingUserNotInProject = new UserAccount { Username = "existingUserNotInProject", Status = UserStatus.Active };
        readonly Project _existingProject = new Project("existing", "existingUser");
        readonly Project _existingProjectNotActive = new Project("existingNotActive", "existingUser") { Status = Status.Cancelled };

        [TestFixtureSetUp]
        public void SetupTests()
        {
            _embeddedDocStore = new EmbeddableDocumentStore {RunInMemory = true};
            _embeddedDocStore.Initialize();
            var documentSession = _embeddedDocStore.OpenSession();
            documentSession.Store(_existingUser);
            documentSession.Store(_existingUserNotInProject);
            documentSession.Store(_existingProject);
            documentSession.Store(_existingProjectNotActive);
            documentSession.SaveChanges();
        }

        [Test]
        public void CreateProjectGoalWithObjectivesSuccess()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            var objectives = new List<Objective> { new Objective("Dancing", "Do A Dance") };
            var goal = new Goal("Big New Goal", objectives, _existingUser.Username);
            pm.AddGoalToProject(_existingProject.Name, goal, _existingUser.Username);
            var session = _embeddedDocStore.OpenSession();
            var project = session.Query<Project>().Where(p => p.Name == _existingProject.Name).First();
            Assert.That(project.Goals.Count(), Is.GreaterThan(0));
            Assert.That(project.Goals[0].Status, Is.EqualTo(Status.Active));
            Assert.That(project.Goals[0].CreatedBy, Is.EqualTo(_existingUser.Username));
            Assert.That(project.Goals[0].Objectives.Count(), Is.GreaterThan(0));
            Assert.That(project.Goals[0].Objectives[0].Name, Is.EqualTo(objectives[0].Name));
            Assert.That(project.Goals[0].Objectives[0].Description, Is.EqualTo(objectives[0].Description));
            Assert.That(project.Goals[0].Objectives[0].PercentageComplete, Is.EqualTo(0));
        }

        [Test]
        public void CreateProjectGoalWithoutObjectivesThrowsException()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            var goal = new Goal("Big New Goal", null, _existingUser.Username);
            const string exceptionMessage = "Goals must contain associated objectives.";
            Assert.That(() => pm.AddGoalToProject(_existingProject.Name, goal, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void CreateProjectGoalUserNotInProjectThrowsException()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            var objectives = new List<Objective> { new Objective("Dancing", "Do A Dance") };
            var goal = new Goal("Big New Goal", objectives, _existingUserNotInProject.Username);
            const string exceptionMessage = "existingUserNotInProject is not authorised to edit Project existing.";
            Assert.That(() => pm.AddGoalToProject(_existingProject.Name, goal, _existingUserNotInProject.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void CreateProjectGoalProjectNotActiveThrowsException()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            var objectives = new List<Objective> { new Objective("Dancing", "Do A Dance") };
            var goal = new Goal("Big New Goal", objectives, _existingUser.Username);
            var exceptionMessage = string.Format("Cannot add stream to project with status of {0}.", _existingProjectNotActive.Status);
            Assert.That(() => pm.AddGoalToProject(_existingProjectNotActive.Name, goal, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void CreateProjectGoalProjectNameNotFoundThrowsException()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            var objectives = new List<Objective> { new Objective("Dancing", "Do A Dance") };
            var goal = new Goal("Big New Goal", objectives, _existingUser.Username);
            const string exceptionMessage = "Could not find Project notarealproject.";
            Assert.That(() => pm.AddGoalToProject("notarealproject", goal, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));            
        }
    }
}