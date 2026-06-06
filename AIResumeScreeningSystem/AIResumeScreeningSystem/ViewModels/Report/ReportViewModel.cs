using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.Report
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        public string ReportTypeDisplay => ReportType.ToString();
        public ReportFormat Format { get; set; }
        public string FormatDisplay => Format.ToString();
        public string? FilePath { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string GeneratedByName { get; set; } = string.Empty;
        public string? JobTitle { get; set; }

        public string FormatIcon => Format switch
        {
            ReportFormat.Excel => "bi-file-earmark-excel text-success",
            ReportFormat.PDF => "bi-file-earmark-pdf text-danger",
            _ => "bi-file-earmark text-secondary"
        };

        public string TypeBadgeClass => ReportType switch
        {
            ReportType.CandidateReport => "bg-primary bg-opacity-10 text-primary",
            ReportType.JobReport => "bg-success bg-opacity-10 text-success",
            ReportType.AIRankingReport => "bg-warning bg-opacity-10 text-warning",
            _ => "bg-secondary bg-opacity-10 text-secondary"
        };
    }
}