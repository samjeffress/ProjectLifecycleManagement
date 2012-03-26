using System;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;

namespace ProjectManagement
{
    public interface IProjectManager
    {
        Project CreateProject(string projectName, UserAccount user);

        void AddProjectStream(Project activeProject, ProjectStream newProjectStream, string username);

        void AddUserToProject(string projectName, string userToAdd, string actingUser);
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
            var project = new Project(projectName, user.Username);
            session.Store(project);
            session.SaveChanges();
            return project;
        }

        public void AddProjectStream(Project activeProject, ProjectStream newProjectStream, string username)
        {
            var session = DocumentStore.OpenSession();
            var project = session.Query<Project>().Where(p => p.Name == activeProject.Name).FirstOrDefault();
            if (project.Status != ProjectStatus.Active)
                throw new ArgumentException(string.Format("Cannot add stream to project with status of {0}.", project.Status));
            if (!project.Users.Contains(username))
                throw new ArgumentException(string.Format("User {0} is currently not active in Project {1}.", username, project.Name));
            project.ProjectStreams.Add(new ProjectStream(newProjectStream.Name, newProjectStream.Description) { CreatedBy = username });
            //session.Store(project);
            session.SaveChanges();
        }

        public void AddUserToProject(string projectName, string userToAdd, string actingUser)
        {
            var session = DocumentStore.OpenSession();
            var project = session.Query<Project>().Where(p => p.Name == projectName).FirstOrDefault();
            if (project == null)
                throw new ArgumentException(string.Format("Could not find Project {0}.", projectName));
            if (!project.Users.Contains(actingUser))
                throw new ArgumentException(string.Format("{0} is not authorised to add new users to Project {1}.", actingUser, projectName));
            var userAccount = session.Query<UserAccount>().Where(u => u.Username == userToAdd).FirstOrDefault();
            if (userAccount == null)
                throw new ArgumentException(string.Format("Could not find UserAccount {0}.", userToAdd));
            if (userAccount.Status != UserStatus.Active)
                throw new ArgumentException(string.Format("{0} is not currently an active user, please reactivate and try again", userToAdd));
            if (!project.Users.Contains(userToAdd))
                project.Users.Add(userToAdd);

            session.SaveChanges();
        }
    }
}
