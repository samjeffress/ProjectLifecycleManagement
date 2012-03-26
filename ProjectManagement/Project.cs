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
            Status = ProjectManagement.Status.Active;
            Users = new List<string> { owner };
            ProjectStreams = new List<ProjectStream>();
            Goals = new List<Goal>();
        }

        public Project()
        {
            Users = new List<string>();
            ProjectStreams = new List<ProjectStream>();
        }

        public Status Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Owner { get; set; }

        public string Name { get; set; }

        public List<string> Users { get; set; }

        public List<ProjectStream> ProjectStreams { get; set; }

        public List<Goal> Goals { get; set; }
    }

    public enum Status
    {
        Active,
        Cancelled,
        Complete
    }
}