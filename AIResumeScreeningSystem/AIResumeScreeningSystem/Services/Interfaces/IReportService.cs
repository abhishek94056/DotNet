using AIResumeScreeningSystem.ViewModels.Report;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface IReportService
    {
        Task<ReportListViewModel> GetReportsAsync(string userId);
        Task<(bool Success, string FilePath, string FileName, string Error)>
            GenerateReportAsync(GenerateReportViewModel model, string userId);
        Task<(bool Success, string Error)> DeleteReportAsync(int reportId, string userId);
        byte[]? GetReportBytes(string filePath);
        string GetContentType(string filePath);
    }
}