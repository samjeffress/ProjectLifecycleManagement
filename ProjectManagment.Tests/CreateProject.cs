using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectManagement;
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
            documentSession.Store(new Project("existing", ""));
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

    [TestFixture]
    public class AddUsersToProject
    {
        private EmbeddableDocumentStore _embeddedDocStore;

        readonly UserAccount _existingUser = new UserAccount { Username = "existingUser", Status = UserStatus.Active };
        readonly UserAccount _existingUserAlreadyAddedToProject = new UserAccount { Username = "existingUserAlreadyAddedToProject", Status = UserStatus.Active };
        readonly UserAccount _notInProjectUser = new UserAccount { Username = "notInProjectUser", Status = UserStatus.Active };
        readonly UserAccount _newActiveUser = new UserAccount { Username = "newActiveUser", Status = UserStatus.Active };
        readonly UserAccount _newInactiveUser = new UserAccount { Username = "newInactiveUser", Status = UserStatus.Inactive };
        readonly Project _existingProject = new Project("existing", "existingUser");
        readonly Project _existingProjectWithExtraUser = new Project("existingWithExtraUser", "existingUser") { Users = new List<string> { "existingUser", "existingUserAlreadyAddedToProject" } };

        [SetUp]
        public void SetupTests()
        {
            _embeddedDocStore = new EmbeddableDocumentStore { RunInMemory = true };
            _embeddedDocStore.Initialize();
            var documentSession = _embeddedDocStore.OpenSession();
            documentSession.Store(_existingUser);
            documentSession.Store(_existingUserAlreadyAddedToProject);
            documentSession.Store(_notInProjectUser);
            documentSession.Store(_newActiveUser);
            documentSession.Store(_newInactiveUser);
            documentSession.Store(_existingProject);
            documentSession.Store(_existingProjectWithExtraUser);
            documentSession.SaveChanges();
        }

        [Test]
        public void AddUserToProjectSuccess()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            pm.AddUserToProject(_existingProject.Name, _newActiveUser.Username, _existingUser.Username);
            var session = _embeddedDocStore.OpenSession();
            var updatedProject = session.Query<Project>().Where(p => p.Name == _existingProject.Name).First();
            Assert.IsTrue(updatedProject.Users.Contains(_newActiveUser.Username));
        }

        [Test]
        public void AddUserToProjectAlreadyExistingSuccess()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            pm.AddUserToProject(_existingProjectWithExtraUser.Name, _existingUserAlreadyAddedToProject.Username, _existingUser.Username);
            var session = _embeddedDocStore.OpenSession();
            var updatedProject = session.Query<Project>().Where(p => p.Name == _existingProjectWithExtraUser.Name).First();
            Assert.IsTrue(updatedProject.Users.Contains(_existingUserAlreadyAddedToProject.Username));
        }

        [Test]
        public  void AddInactiveUserToProjectThrowsException()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            var exceptionMessage = string.Format("{0} is not currently an active user, please reactivate and try again", _newInactiveUser.Username);
            Assert.That(() => pm.AddUserToProject(_existingProject.Name, _newInactiveUser.Username, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void AddNonExistentUserToProjectThrowsException()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            const string userToAdd = "doesntexist";
            var exceptionMessage = string.Format("Could not find UserAccount {0}.", userToAdd);
            Assert.That(() => pm.AddUserToProject(_existingProject.Name, userToAdd, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void AddUserToProjectActingUserNotInProjectThrowsException()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            string exceptionMessage = string.Format("{0} is not authorised to add new users to Project {1}.", _notInProjectUser.Username, _existingProject.Name);
            Assert.That(() => pm.AddUserToProject(_existingProject.Name, _newActiveUser.Username, _notInProjectUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void AddUserToProject_ProjectNotFoundThrowsException()
        {
            var pm = new ProjectManager { DocumentStore = _embeddedDocStore };
            var notExistingProjectName = "notExistingProject";
            string exceptionMessage = string.Format("Could not find Project {0}.", notExistingProjectName);
            Assert.That(() => pm.AddUserToProject(notExistingProjectName, _newActiveUser.Username, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }
    }
}
