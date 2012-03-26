using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectManagement;
using Raven.Client.Embedded;
using Raven.Client.Linq;

namespace ProjectManagment.Tests
{
    [TestFixture]
    public class AddUsersToProjectTestFixture
    {
        private EmbeddableDocumentStore _embeddedDocStore;

        readonly UserAccount _existingUser = new UserAccount { Username = "existingUser", Status = UserStatus.Active };
        readonly UserAccount _existingUserAlreadyAddedToProject = new UserAccount { Username = "existingUserAlreadyAddedToProject", Status = UserStatus.Active };
        readonly UserAccount _notInProjectUser = new UserAccount { Username = "notInProjectUser", Status = UserStatus.Active };
        readonly UserAccount _newActiveUser = new UserAccount { Username = "newActiveUser", Status = UserStatus.Active };
        readonly UserAccount _newInactiveUser = new UserAccount { Username = "newInactiveUser", Status = UserStatus.Inactive };
        readonly Project _existingProject = new Project("existing", "existingUser");
        readonly Project _existingProjectWithExtraUser = new Project("existingWithExtraUser", "existingUser") { Users = new List<string> { "existingUser", "existingUserAlreadyAddedToProject" } };

        [TestFixtureSetUp]
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
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            pm.AddUserToProject(_existingProject.Name, _newActiveUser.Username, _existingUser.Username);
            var session = _embeddedDocStore.OpenSession();
            var updatedProject = session.Query<Project>().Where(p => p.Name == _existingProject.Name).First();
            Assert.IsTrue(updatedProject.Users.Contains(_newActiveUser.Username));
        }

        [Test]
        public void AddUserToProjectAlreadyExistingSuccess()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            pm.AddUserToProject(_existingProjectWithExtraUser.Name, _existingUserAlreadyAddedToProject.Username, _existingUser.Username);
            var session = _embeddedDocStore.OpenSession();
            var updatedProject = session.Query<Project>().Where(p => p.Name == _existingProjectWithExtraUser.Name).First();
            Assert.IsTrue(updatedProject.Users.Contains(_existingUserAlreadyAddedToProject.Username));
        }

        [Test]
        public  void AddInactiveUserToProjectThrowsException()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            var exceptionMessage = string.Format("{0} is not currently an active user, please reactivate and try again", _newInactiveUser.Username);
            Assert.That(() => pm.AddUserToProject(_existingProject.Name, _newInactiveUser.Username, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void AddNonExistentUserToProjectThrowsException()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            const string userToAdd = "doesntexist";
            var exceptionMessage = string.Format("Could not find UserAccount {0}.", userToAdd);
            Assert.That(() => pm.AddUserToProject(_existingProject.Name, userToAdd, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void AddUserToProjectActingUserNotInProjectThrowsException()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            var exceptionMessage = string.Format("{0} is not authorised to edit Project {1}.", _notInProjectUser.Username, _existingProject.Name);
            Assert.That(() => pm.AddUserToProject(_existingProject.Name, _newActiveUser.Username, _notInProjectUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void AddUserToProject_ProjectNotFoundThrowsException()
        {
            var pm = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            const string notExistingProjectName = "notExistingProject";
            var exceptionMessage = string.Format("Could not find Project {0}.", notExistingProjectName);
            Assert.That(() => pm.AddUserToProject(notExistingProjectName, _newActiveUser.Username, _existingUser.Username), Throws.ArgumentException.With.Message.EqualTo(exceptionMessage));
        }
    }
}