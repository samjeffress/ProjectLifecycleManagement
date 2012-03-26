using System;
using System.Collections.Generic;

namespace ProjectManagement
{
    public class Project
    {
        public Project(string projectName, string owner)
        {
            Owner = owner;
            Name = projectName;
            CreatedDate = DateTime.Now;
            Status = ProjectStatus.Active;
            Users = new List<string> { owner };
            ProjectStreams = new List<ProjectStream>();
        }

        public Project()
        {
            Users = new List<string>();
            ProjectStreams = new List<ProjectStream>();
        }

        public ProjectStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Owner { get; set; }

        public string Name { get; set; }

        public List<string> Users { get; set; }

        public List<ProjectStream> ProjectStreams { get; set; }
    }

    public enum ProjectStatus
    {
        Active,
        Cancelled,
        Complete
    }

    public class ProjectStream
    {
        public ProjectStream(string name, string description)
        {
            Name = name;
            Description = description;
            CreatedDate = DateTime.Now;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }
    }
}