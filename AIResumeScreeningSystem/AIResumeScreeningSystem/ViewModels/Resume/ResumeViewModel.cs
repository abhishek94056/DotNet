using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.Resume
{
    public class ResumeViewModel
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public ResumeStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public DateTime UploadedAt { get; set; }
        public DateTime? ParsedAt { get; set; }
        public bool IsActive { get; set; }

        // Parsed data
        public string? ParsedName { get; set; }
        public string? ParsedEmail { get; set; }
        public string? ParsedPhone { get; set; }
        public string? ParsedSkills { get; set; }
        public string? ParsedEducation { get; set; }
        public string? ParsedExperience { get; set; }
        public string? ParsedSummary { get; set; }

        // Computed
        public string FileSizeDisplay => FileSizeBytes switch
        {
            < 1024 => $"{FileSizeBytes} B",
            < 1024 * 1024 => $"{FileSizeBytes / 1024.0:F1} KB",
            _ => $"{FileSizeBytes / (1024.0 * 1024):F1} MB"
        };

        public string FileIconClass => FileExtension.ToLower() switch
        {
            ".pdf" => "bi-file-earmark-pdf text-danger",
            ".docx" or ".doc" => "bi-file-earmark-word text-primary",
            _ => "bi-file-earmark text-secondary"
        };

        public string StatusBadgeClass => Status switch
        {
            ResumeStatus.Uploaded => "bg-secondary",
            ResumeStatus.Parsing => "bg-warning text-dark",
            ResumeStatus.Parsed => "bg-success",
            ResumeStatus.Failed => "bg-danger",
            _ => "bg-secondary"
        };

        public List<string> ParsedSkillsList =>
            string.IsNullOrEmpty(ParsedSkills)
                ? new List<string>()
                : ParsedSkills.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(s => s.Trim())
                              .Where(s => !string.IsNullOrEmpty(s))
                              .ToList();
    }
}