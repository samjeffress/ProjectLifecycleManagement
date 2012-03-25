using System;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;

namespace ProjectManagement
{
    public interface IProjectManager
    {
        Project CreateProject(string projectName, UserAccount user);
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
