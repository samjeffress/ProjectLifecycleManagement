using System.Linq;
using NUnit.Framework;
using ProjectManagement;
using Raven.Client.Embedded;
using Raven.Client.Linq;

namespace ProjectManagment.Tests
{
    [TestFixture]
    public class CreateUserAccountTestFixture
    {
        private EmbeddableDocumentStore _embeddedDocStore;

        [TestFixtureSetUp]
        public void SetupTests()
        {
            _embeddedDocStore = new EmbeddableDocumentStore { RunInMemory = true };
            _embeddedDocStore.Initialize();
            var documentSession = _embeddedDocStore.OpenSession();
            documentSession.Store(new UserAccount { Username = "existingUser", Status = UserStatus.Active });
            documentSession.SaveChanges();
        }

        [Test]
        public void CreateUserNotExistingSuccess()
        {
            var projectManager = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            const string newUser = "newUser";
            projectManager.CreateUserAccount(newUser);
            var session = _embeddedDocStore.OpenSession();
            var userCount = session.Query<UserAccount>().Where(u => u.Username == newUser && u.Status == UserStatus.Active).Count();
            Assert.That(userCount, Is.EqualTo(1));
        }

        [Test]
        public void CreateUserAlreadyExistsThrowsException()
        {
            var projectManager = new ProjectManager { Session = _embeddedDocStore.OpenSession() };
            const string newUser = "existingUser";
            Assert.That(() => projectManager.CreateUserAccount(newUser), Throws.ArgumentException.With.Message.EqualTo("User " + newUser + " already exists."));
        }
    }
}