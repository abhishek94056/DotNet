using AIResumeScreeningSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AIResumeScreeningSystem.ViewModels.Report
{
    public class GenerateReportViewModel
    {
        [Required]
        [Display(Name = "Report Type")]
        public ReportType ReportType { get; set; } = ReportType.CandidateReport;

        [Required]
        [Display(Name = "Export Format")]
        public ReportFormat Format { get; set; } = ReportFormat.Excel;

        [Display(Name = "Job (for Job/Ranking reports)")]
        public int? JobId { get; set; }

        [Display(Name = "Date From")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Include AI Scores")]
        public bool IncludeAIScores { get; set; } = true;

        [Display(Name = "Include Skill Analysis")]
        public bool IncludeSkillAnalysis { get; set; } = true;

        [Display(Name = "Top Candidates Only (min score %)")]
        [Range(0, 100)]
        public int? MinScoreFilter { get; set; }

        // Dropdowns
        public List<SelectListItem> AvailableJobs { get; set; } = new();

        public IEnumerable<SelectListItem> ReportTypeOptions =>
            Enum.GetValues<ReportType>()
                .Select(rt => new SelectListItem(
                    rt.ToString().Replace("Report", " Report"), ((int)rt).ToString()));

        public IEnumerable<SelectListItem> FormatOptions =>
            Enum.GetValues<ReportFormat>()
                .Select(rf => new SelectListItem(rf.ToString(), ((int)rf).ToString()));
    }
}