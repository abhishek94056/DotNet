namespace AIResumeScreeningSystem.ViewModels.AI
{
    public class ResumeSummaryViewModel
    {
        public int ResumeId { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        // AI-generated content
        public string? ExecutiveSummary { get; set; }
        public string? TechnicalProfile { get; set; }
        public string? CareerHighlights { get; set; }
        public string? EducationSummary { get; set; }
        public string? ImprovementSuggestions { get; set; }

        // Structured parsed data
        public List<string> TopSkills { get; set; } = new();
        public List<string> Achievements { get; set; } = new();
        public List<string> ImprovementTips { get; set; } = new();

        // Scores assigned by AI
        public int ResumeQualityScore { get; set; }
        public int ContentCompletenessScore { get; set; }
        public int PresentationScore { get; set; }

        public string QualityBadgeClass => ResumeQualityScore switch
        {
            >= 80 => "bg-success",
            >= 60 => "bg-primary",
            >= 40 => "bg-warning text-dark",
            _ => "bg-danger"
        };

        public bool IsLoaded { get; set; }
        public string? Error { get; set; }
    }
}