using System;
using System.Collections.Generic;

namespace ProjectManagement
{
    public class Goal
    {
        public Goal(string goalName, List<Objective> objectives, string username)
        {
            Name = goalName;
            CreatedBy = username;
            CreatedDate = DateTime.Now;
            Status = Status.Active;
            Objectives = objectives;
        }

        public Goal()
        {
            Objectives = new List<Objective>();
        }

        public string CreatedBy { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public Status Status { get; set; }

        public List<Objective> Objectives { get; set; }
    }
}