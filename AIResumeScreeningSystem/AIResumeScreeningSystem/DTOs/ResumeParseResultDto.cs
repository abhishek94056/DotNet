namespace AIResumeScreeningSystem.DTOs
{
    public class ResumeParseResultDto
    {
        public bool Success { get; set; }
        public string? Error { get; set; }

        // Extracted fields
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Summary { get; set; }

        // Raw extracted text sections
        public string? RawText { get; set; }
        public string? SkillsSection { get; set; }
        public string? EducationSection { get; set; }
        public string? ExperienceSection { get; set; }

        // Parsed structured lists
        public List<string> ExtractedSkills { get; set; } = new();
        public List<string> ExtractedEducation { get; set; } = new();
        public List<string> ExtractedExperience { get; set; } = new();

        // Inferred fields
        public int EstimatedExperienceYears { get; set; }
        public string? HighestEducation { get; set; }
    }
}