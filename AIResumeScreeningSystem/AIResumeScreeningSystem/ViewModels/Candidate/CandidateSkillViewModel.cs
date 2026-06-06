using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.Candidate
{
    public class CandidateSkillViewModel
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public SkillCategory Category { get; set; }
        public string CategoryDisplay => Category.ToString();
        public ProficiencyLevel ProficiencyLevel { get; set; }
        public string ProficiencyDisplay => ProficiencyLevel.ToString();
        public int YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }

        public int ProficiencyPercent => ProficiencyLevel switch
        {
            ProficiencyLevel.Beginner => 25,
            ProficiencyLevel.Intermediate => 50,
            ProficiencyLevel.Advanced => 75,
            ProficiencyLevel.Expert => 100,
            _ => 0
        };

        public string ProficiencyBarClass => ProficiencyLevel switch
        {
            ProficiencyLevel.Beginner => "bg-danger",
            ProficiencyLevel.Intermediate => "bg-warning",
            ProficiencyLevel.Advanced => "bg-info",
            ProficiencyLevel.Expert => "bg-success",
            _ => "bg-secondary"
        };
    }
}