using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.Candidate
{
    public class CandidateViewModel
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
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? HighestEducation { get; set; }
        public string? University { get; set; }
        public int? GraduationYear { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public string? ProfileImagePath { get; set; }
        public DateTime CreatedAt { get; set; }

        // Skills
        public List<CandidateSkillViewModel> Skills { get; set; } = new();

        // Stats
        public int TotalApplications { get; set; }
        public int TotalResumes { get; set; }
        public int ShortlistedCount { get; set; }

        // Computed
        public string InitialsAvatar => FullName.Length >= 2
            ? $"{FullName.Split(' ').First()[0]}{FullName.Split(' ').Last()[0]}".ToUpper()
            : FullName[..1].ToUpper();

        public string ExperienceLabel => TotalExperienceYears switch
        {
            0 => "Fresher",
            <= 2 => "Junior",
            <= 5 => "Mid-level",
            <= 10 => "Senior",
            _ => "Expert"
        };

        public string AvailabilityBadgeClass =>
            IsAvailable ? "bg-success bg-opacity-10 text-success" : "bg-secondary bg-opacity-10 text-secondary";
    }
}