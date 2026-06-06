using AIResumeScreeningSystem.DTOs;
using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.SkillMatching
{
    public class SkillMatchResultViewModel
    {
        // Application & Job info
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;

        // Candidate info
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? CandidateHeadline { get; set; }
        public int CandidateExperienceYears { get; set; }
        public string? CandidateEducation { get; set; }

        // Match scores
        public decimal OverallMatchScore { get; set; }
        public decimal RequiredSkillScore { get; set; }
        public decimal OptionalSkillScore { get; set; }
        public decimal ExperienceScore { get; set; }
        public decimal EducationScore { get; set; }

        // Skill breakdown
        public List<string> MatchedRequiredSkills { get; set; } = new();
        public List<string> MatchedOptionalSkills { get; set; } = new();
        public List<string> MissingRequiredSkills { get; set; } = new();
        public List<string> MissingOptionalSkills { get; set; } = new();
        public List<string> CandidateExtraSkills { get; set; } = new();

        // Counts
        public int TotalRequiredSkills { get; set; }
        public int TotalOptionalSkills { get; set; }

        // Application status
        public ApplicationStatus Status { get; set; }
        public int? RankPosition { get; set; }
        public string? AIEvaluation { get; set; }
        public string? SkillGapAnalysis { get; set; }
        public string? RecruiterNotes { get; set; }
        public DateTime AppliedAt { get; set; }

        // Computed helpers
        public string MatchGrade => OverallMatchScore switch
        {
            >= 90 => "Excellent",
            >= 75 => "Strong",
            >= 60 => "Good",
            >= 45 => "Fair",
            >= 30 => "Weak",
            _ => "Poor"
        };

        public string GradeBadgeClass => OverallMatchScore switch
        {
            >= 90 => "bg-success",
            >= 75 => "bg-primary",
            >= 60 => "bg-info text-dark",
            >= 45 => "bg-warning text-dark",
            _ => "bg-danger"
        };

        public string ScoreRingColor => OverallMatchScore switch
        {
            >= 75 => "#059669",
            >= 50 => "#1a56db",
            >= 30 => "#d97706",
            _ => "#dc2626"
        };

        public string StatusBadgeClass => Status switch
        {
            ApplicationStatus.Submitted => "bg-secondary",
            ApplicationStatus.UnderReview => "bg-info text-dark",
            ApplicationStatus.Shortlisted => "bg-warning text-dark",
            ApplicationStatus.InterviewScheduled => "bg-primary",
            ApplicationStatus.Approved => "bg-success",
            ApplicationStatus.Rejected => "bg-danger",
            ApplicationStatus.Withdrawn => "bg-secondary",
            _ => "bg-secondary"
        };

        public int RequiredMatchPercent =>
            TotalRequiredSkills > 0
                ? (int)Math.Round((double)MatchedRequiredSkills.Count / TotalRequiredSkills * 100)
                : 0;

        public int OptionalMatchPercent =>
            TotalOptionalSkills > 0
                ? (int)Math.Round((double)MatchedOptionalSkills.Count / TotalOptionalSkills * 100)
                : 0;
    }
}