namespace AIResumeScreeningSystem.DTOs
{
    public class CandidateDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? CurrentJobTitle { get; set; }
        public string? CurrentCompany { get; set; }
        public int TotalExperienceYears { get; set; }
        public string? Location { get; set; }
        public string? HighestEducation { get; set; }
        public string? University { get; set; }
        public int? GraduationYear { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> Skills { get; set; } = new();
        public int TotalApplications { get; set; }
        public int TotalResumes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}