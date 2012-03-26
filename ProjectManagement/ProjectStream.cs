using System;

namespace ProjectManagement
{
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