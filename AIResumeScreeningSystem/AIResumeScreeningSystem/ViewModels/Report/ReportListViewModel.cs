namespace AIResumeScreeningSystem.ViewModels.Report
{
    public class ReportListViewModel
    {
        public List<ReportViewModel> Reports { get; set; } = new();
        public int TotalCount => Reports.Count;
        public int ExcelCount => Reports.Count(r => r.Format == Models.ReportFormat.Excel);
        public int PdfCount => Reports.Count(r => r.Format == Models.ReportFormat.PDF);
    }
}