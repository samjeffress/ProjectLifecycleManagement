using System;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;

namespace ProjectManagement
{
    public interface IProjectManager
    {
        Project CreateProject(string projectName, string user);

        void AddProjectStream(string activeProjectNameName, ProjectStream newProjectStream, string username);

        void AddUserToProject(string projectName, string userToAdd, string actingUser);

        void AddGoalToProject(string projectName, Goal goal, string username);
    }

    public class ProjectManager : IProjectManager
    {
        public IDocumentSession Session { get; set; }

        public Project CreateProject(string projectName, string user)
        {
            var inactiveUserAccount = Session.Query<UserAccount>().Where(u => u.Username == user && u.Status == UserStatus.Active).Count();
            if (inactiveUserAccount != 1)
                throw new ArgumentException(string.Format("Cannot create new project '{0}' - {1} is an inactive user.", projectName, user));
            var existingProjects = Session.Query<Project>().Where(p => p.Name == projectName).Count();
            if (existingProjects > 0)
                throw new ArgumentException(string.Format("Cannot create new project '{0}' - this project name is already in use.", projectName));
            var project = new Project(projectName, user);
            Session.Store(project);
            Session.SaveChanges();
            return project;
        }

        public void AddProjectStream(string activeProjectName, ProjectStream newProjectStream, string username)
        {
            var project = Session.Query<Project>().Where(p => p.Name == activeProjectName).FirstOrDefault();
            if (project.Status != Status.Active)
                throw new ArgumentException(string.Format("Cannot add stream to project with status of {0}.", project.Status));
            if (!project.Users.Contains(username))
                throw new ArgumentException(string.Format("User {0} is currently not active in Project {1}.", username, project.Name));
            project.ProjectStreams.Add(new ProjectStream(newProjectStream.Name, newProjectStream.Description) { CreatedBy = username });
            Session.SaveChanges();
        }

        public void AddUserToProject(string projectName, string userToAdd, string actingUser)
        {
            var project = Session.Query<Project>().Where(p => p.Name == projectName).FirstOrDefault();
            ValidateProjectAndUser(projectName, project, actingUser);
            var userAccount = Session.Query<UserAccount>().Where(u => u.Username == userToAdd).FirstOrDefault();
            if (userAccount == null)
                throw new ArgumentException(string.Format("Could not find UserAccount {0}.", userToAdd));
            if (userAccount.Status != UserStatus.Active)
                throw new ArgumentException(string.Format("{0} is not currently an active user, please reactivate and try again", userToAdd));
            if (!project.Users.Contains(userToAdd))
                project.Users.Add(userToAdd);

            Session.SaveChanges();
        }

        public void AddGoalToProject(string projectName, Goal goal, string username)
        {
            var project = Session.Query<Project>().Where(p => p.Name == projectName).FirstOrDefault();
            ValidateProjectAndUser(projectName, project, username);
            if (project.Status != Status.Active)
                throw new ArgumentException(string.Format("Cannot add stream to project with status of {0}.", project.Status));
            if (goal.Objectives == null || goal.Objectives.Count() == 0)
                throw new ArgumentException("Goals must contain associated objectives.");
            project.Goals.Add(goal);
            Session.SaveChanges();
        }

        private void ValidateProjectAndUser(string projectName, Project project, string actingUser)
        {
            if (project == null)
                throw new ArgumentException(string.Format("Could not find Project {0}.", projectName));
            if (!project.Users.Contains(actingUser))
                throw new ArgumentException(string.Format("{0} is not authorised to edit Project {1}.", actingUser, projectName));
        }
    }
}
