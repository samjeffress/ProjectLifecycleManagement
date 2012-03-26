namespace ProjectManagement
{
    public class Objective
    {
        public Objective(string objectiveName, string objectiveDescription)
        {
            Name = objectiveName;
            Description = objectiveDescription;
            PercentageComplete = 0; 
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal PercentageComplete { get; set; }
    }
}