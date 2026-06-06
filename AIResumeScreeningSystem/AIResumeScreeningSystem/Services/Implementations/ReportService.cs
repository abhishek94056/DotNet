using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Report;
using ClosedXML.Excel;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            AppDbContext context,
            IWebHostEnvironment env,
            ILogger<ReportService> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // ─── Report List ───────────────────────────────────────────────────

        public async Task<ReportListViewModel> GetReportsAsync(string userId)
        {
            var reports = await _context.Reports
                .Include(r => r.GeneratedBy)
                .Include(r => r.Job)
                .Where(r => r.GeneratedByUserId == userId)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();

            return new ReportListViewModel
            {
                Reports = reports.Select(r => new ReportViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    ReportType = r.ReportType,
                    Format = r.Format,
                    FilePath = r.FilePath,
                    GeneratedAt = r.GeneratedAt,
                    GeneratedByName =
                        $"{r.GeneratedBy.FirstName} {r.GeneratedBy.LastName}",
                    JobTitle = r.Job?.Title
                }).ToList()
            };
        }

        // ─── Generate ──────────────────────────────────────────────────────

        public async Task<(bool Success, string FilePath, string FileName, string Error)>
            GenerateReportAsync(GenerateReportViewModel model, string userId)
        {
            try
            {
                var reportsFolder = Path.Combine(
                    _env.WebRootPath, "uploads", "reports");
                Directory.CreateDirectory(reportsFolder);

                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var typeName = model.ReportType.ToString().Replace("Report", "");
                var ext = model.Format == ReportFormat.Excel ? ".xlsx" : ".pdf";
                var fileName = $"{typeName}_Report_{timestamp}{ext}";
                var fullPath = Path.Combine(reportsFolder, fileName);
                var relativePath = $"/uploads/reports/{fileName}";

                bool success = model.Format switch
                {
                    ReportFormat.Excel => await GenerateExcelAsync(
                        model, fullPath),
                    ReportFormat.PDF => await GeneratePdfAsync(
                        model, fullPath),
                    _ => false
                };

                if (!success)
                    return (false, string.Empty, string.Empty, "Report generation failed.");

                // Save report record
                var reportName =
                    $"{typeName} Report — {DateTime.UtcNow:MMM dd, yyyy HH:mm}";
                var dbReport = new Report
                {
                    Name = reportName,
                    ReportType = model.ReportType,
                    Format = model.Format,
                    FilePath = relativePath,
                    GeneratedAt = DateTime.UtcNow,
                    GeneratedByUserId = userId,
                    JobId = model.JobId
                };
                await _context.Reports.AddAsync(dbReport);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Report generated: {FileName} by {UserId}", fileName, userId);

                return (true, relativePath, fileName, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                return (false, string.Empty, string.Empty,
                    $"Error: {ex.Message}");
            }
        }

        // ─── Excel Generation ──────────────────────────────────────────────

        private async Task<bool> GenerateExcelAsync(
            GenerateReportViewModel model, string filePath)
        {
            using var workbook = new XLWorkbook();

            switch (model.ReportType)
            {
                case ReportType.CandidateReport:
                    await BuildCandidateExcelSheetAsync(workbook, model);
                    break;
                case ReportType.JobReport:
                    await BuildJobExcelSheetAsync(workbook, model);
                    break;
                case ReportType.AIRankingReport:
                    await BuildRankingExcelSheetAsync(workbook, model);
                    break;
            }

            workbook.SaveAs(filePath);
            return true;
        }

        private async Task BuildCandidateExcelSheetAsync(
            IXLWorkbook workbook, GenerateReportViewModel model)
        {
            var ws = workbook.Worksheets.Add("Candidates");

            // Header Row Styling
            var headerRow = ws.Row(1);
            headerRow.Height = 20;
            var headerStyle = workbook.Style;

            string[] headers =
            {
                "ID", "Full Name", "Email", "Phone", "Location",
                "Experience (yrs)", "Education", "Total Skills",
                "Total Applications", "Shortlisted", "Created"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1a56db");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var candidates = await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills)
                .Include(c => c.Applications)
                .Where(c => c.User.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            if (model.DateFrom.HasValue)
                candidates = candidates
                    .Where(c => c.CreatedAt >= model.DateFrom.Value).ToList();
            if (model.DateTo.HasValue)
                candidates = candidates
                    .Where(c => c.CreatedAt <= model.DateTo.Value).ToList();

            for (int row = 0; row < candidates.Count; row++)
            {
                var c = candidates[row];
                var rowNum = row + 2;
                var bgColor = row % 2 == 0
                    ? XLColor.White : XLColor.FromHtml("#f0f4ff");

                var values = new object[]
                {
                    c.Id,
                    $"{c.User.FirstName} {c.User.LastName}",
                    c.User.Email ?? "",
                    c.User.PhoneNumber ?? "",
                    c.Location ?? "",
                    c.TotalExperienceYears,
                    c.HighestEducation ?? "",
                    c.CandidateSkills.Count,
                    c.Applications.Count,
                    c.Applications.Count(a => a.Status == ApplicationStatus.Shortlisted),
                    c.CreatedAt.ToString("yyyy-MM-dd")
                };

                for (int col = 0; col < values.Length; col++)
                {
                    var cell = ws.Cell(rowNum, col + 1);
                    cell.Value = values[col]?.ToString() ?? "";
                    cell.Style.Fill.BackgroundColor = bgColor;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Hair;
                }
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            // Summary sheet
            var summary = workbook.Worksheets.Add("Summary");
            summary.Cell("A1").Value = "AI Resume Screening — Candidate Report";
            summary.Cell("A1").Style.Font.Bold = true;
            summary.Cell("A1").Style.Font.FontSize = 14;
            summary.Cell("A3").Value = "Generated At:";
            summary.Cell("B3").Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
            summary.Cell("A4").Value = "Total Candidates:";
            summary.Cell("B4").Value = candidates.Count;
            summary.Cell("A5").Value = "Available:";
            summary.Cell("B5").Value = candidates.Count(c => c.IsAvailable);
            summary.Columns().AdjustToContents();
        }

        private async Task BuildJobExcelSheetAsync(
            IXLWorkbook workbook, GenerateReportViewModel model)
        {
            var ws = workbook.Worksheets.Add("Jobs");

            string[] headers =
            {
                "ID", "Title", "Company", "Type", "Status", "Location",
                "Salary Min", "Salary Max", "Applications", "Shortlisted",
                "Avg Match Score", "Posted Date", "Expiry Date"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#059669");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var jobsQuery = _context.Jobs
                .Include(j => j.Applications);

            if (model.JobId.HasValue)
                jobsQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Job, System.Collections.Generic.ICollection<Application>>)
                    jobsQuery.Where(j => j.Id == model.JobId.Value);

            var jobs = await jobsQuery
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            if (model.DateFrom.HasValue)
                jobs = jobs.Where(j => j.PostedDate >= model.DateFrom.Value).ToList();
            if (model.DateTo.HasValue)
                jobs = jobs.Where(j => j.PostedDate <= model.DateTo.Value).ToList();

            for (int row = 0; row < jobs.Count; row++)
            {
                var j = jobs[row];
                var rowNum = row + 2;
                var bgColor = row % 2 == 0
                    ? XLColor.White : XLColor.FromHtml("#f0fff4");

                var avgScore = j.Applications.Any(a => a.AIMatchScore.HasValue)
                    ? j.Applications
                        .Where(a => a.AIMatchScore.HasValue)
                        .Average(a => a.AIMatchScore!.Value)
                        .ToString("F1")
                    : "N/A";

                var values = new object[]
                {
                    j.Id, j.Title, j.Company, j.JobType.ToString(),
                    j.Status.ToString(), j.Location ?? "",
                    j.SalaryMin?.ToString("N0") ?? "",
                    j.SalaryMax?.ToString("N0") ?? "",
                    j.Applications.Count,
                    j.Applications.Count(a => a.Status == ApplicationStatus.Shortlisted),
                    avgScore,
                    j.PostedDate.ToString("yyyy-MM-dd"),
                    j.ExpiryDate?.ToString("yyyy-MM-dd") ?? ""
                };

                for (int col = 0; col < values.Length; col++)
                {
                    var cell = ws.Cell(rowNum, col + 1);
                    cell.Value = values[col]?.ToString() ?? "";
                    cell.Style.Fill.BackgroundColor = bgColor;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Hair;
                }
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);
        }

        private async Task BuildRankingExcelSheetAsync(
            IXLWorkbook workbook, GenerateReportViewModel model)
        {
            var ws = workbook.Worksheets.Add("AI Ranking");

            string[] headers =
            {
                "Rank", "Candidate", "Email", "Job Title", "AI Match Score",
                "Skill Match %", "Experience (yrs)", "Status",
                "Missing Skills", "Applied Date"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#d97706");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var appsQuery = _context.Applications
                .Include(a => a.Job)
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Where(a => a.AIMatchScore.HasValue);

            if (model.JobId.HasValue)
                appsQuery = appsQuery.Where(a => a.JobId == model.JobId.Value);

            if (model.MinScoreFilter.HasValue)
                appsQuery = appsQuery
                    .Where(a => a.AIMatchScore >= model.MinScoreFilter.Value);

            var applications = await appsQuery
                .OrderByDescending(a => a.AIMatchScore)
                .ToListAsync();

            if (model.DateFrom.HasValue)
                applications = applications
                    .Where(a => a.AppliedAt >= model.DateFrom.Value).ToList();

            for (int row = 0; row < applications.Count; row++)
            {
                var a = applications[row];
                var rowNum = row + 2;

                // Colour-code rows by score
                XLColor bgColor;
                if (a.AIMatchScore >= 75) bgColor = XLColor.FromHtml("#d1fae5");
                else if (a.AIMatchScore >= 50) bgColor = XLColor.FromHtml("#dbeafe");
                else if (a.AIMatchScore >= 30) bgColor = XLColor.FromHtml("#fef9c3");
                else bgColor = XLColor.FromHtml("#fee2e2");

                var values = new object[]
                {
                    row + 1,
                    $"{a.Candidate.User.FirstName} {a.Candidate.User.LastName}",
                    a.Candidate.User.Email ?? "",
                    a.Job?.Title ?? "",
                    $"{a.AIMatchScore:F1}%",
                    $"{a.SkillMatchPercentage:F1}%",
                    a.Candidate.TotalExperienceYears,
                    a.Status.ToString(),
                    a.MissingSkills ?? "",
                    a.AppliedAt.ToString("yyyy-MM-dd")
                };

                for (int col = 0; col < values.Length; col++)
                {
                    var cell = ws.Cell(rowNum, col + 1);
                    cell.Value = values[col]?.ToString() ?? "";
                    cell.Style.Fill.BackgroundColor = bgColor;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Hair;
                    if (col == 4)   // Score column bold
                        cell.Style.Font.Bold = true;
                }
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            // Add a score legend sheet
            var legend = workbook.Worksheets.Add("Legend");
            var legendData = new[]
            {
                ("#d1fae5", "75-100% — Excellent Match"),
                ("#dbeafe", "50-74% — Good Match"),
                ("#fef9c3", "30-49% — Fair Match"),
                ("#fee2e2", "0-29% — Weak Match")
            };
            legend.Cell("A1").Value = "Score Colour Legend";
            legend.Cell("A1").Style.Font.Bold = true;
            for (int i = 0; i < legendData.Length; i++)
            {
                var (hex, label) = legendData[i];
                legend.Cell(i + 2, 1).Style.Fill.BackgroundColor = XLColor.FromHtml(hex);
                legend.Cell(i + 2, 1).Value = label;
                legend.Cell(i + 2, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
            legend.Column(1).Width = 30;
        }

        // ─── PDF Generation ────────────────────────────────────────────────

        private async Task<bool> GeneratePdfAsync(
            GenerateReportViewModel model, string filePath)
        {
            try
            {
                using var writer = new PdfWriter(filePath);
                using var pdf = new PdfDocument(writer);
                var document = new Document(pdf,
                    iText.Kernel.Geom.PageSize.A4.Rotate());
                document.SetMargins(30, 30, 30, 30);

                var titleFont = PdfFontFactory.CreateFont(
                    iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                var bodyFont = PdfFontFactory.CreateFont(
                    iText.IO.Font.Constants.StandardFonts.HELVETICA);

                // ── Cover Header ───────────────────────────────────────────
                var headerTable = new Table(1).UseAllAvailableWidth();
                var headerCell = new Cell()
                    .SetBackgroundColor(new DeviceRgb(26, 86, 219))
                    .SetPadding(20);

                headerCell.Add(new Paragraph("AI Resume Screening System")
                    .SetFont(titleFont)
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.WHITE));

                headerCell.Add(new Paragraph(
                    $"{model.ReportType.ToString().Replace("Report", " Report")} — " +
                    $"Generated {DateTime.UtcNow:dd MMM yyyy HH:mm}")
                    .SetFont(bodyFont)
                    .SetFontSize(11)
                    .SetFontColor(ColorConstants.WHITE));

                headerTable.AddCell(headerCell);
                document.Add(headerTable);
                document.Add(new Paragraph("\n"));

                switch (model.ReportType)
                {
                    case ReportType.CandidateReport:
                        await AddCandidatePdfSectionAsync(
                            document, model, titleFont, bodyFont);
                        break;
                    case ReportType.JobReport:
                        await AddJobPdfSectionAsync(
                            document, model, titleFont, bodyFont);
                        break;
                    case ReportType.AIRankingReport:
                        await AddRankingPdfSectionAsync(
                            document, model, titleFont, bodyFont);
                        break;
                }

                // Footer
                document.Add(new Paragraph(
                    $"\nReport generated by AI Resume Screening System · " +
                    $"{DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC")
                    .SetFont(bodyFont)
                    .SetFontSize(9)
                    .SetFontColor(new DeviceRgb(107, 114, 128))
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Close();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF generation failed");
                return false;
            }
        }

        private async Task AddCandidatePdfSectionAsync(
            Document document, GenerateReportViewModel model,
            PdfFont titleFont, PdfFont bodyFont)
        {
            document.Add(new Paragraph("Candidate Report")
                .SetFont(titleFont).SetFontSize(16)
                .SetFontColor(new DeviceRgb(26, 86, 219)));

            var candidates = await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills)
                .Include(c => c.Applications)
                .Where(c => c.User.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // Summary paragraph
            document.Add(new Paragraph(
                $"Total Candidates: {candidates.Count} · " +
                $"Available: {candidates.Count(c => c.IsAvailable)} · " +
                $"Generated: {DateTime.UtcNow:dd MMM yyyy}")
                .SetFont(bodyFont).SetFontSize(11)
                .SetFontColor(new DeviceRgb(107, 114, 128)));

            document.Add(new Paragraph("\n"));

            // Table
            string[] cols =
            {
                "Name", "Email", "Experience", "Education",
                "Skills", "Applications"
            };
            float[] colWidths = { 2f, 3f, 1.5f, 2f, 1f, 1.5f };

            var table = new Table(UnitValue.CreatePercentArray(colWidths))
                .UseAllAvailableWidth();

            // Header
            foreach (var col in cols)
            {
                table.AddHeaderCell(
                    new Cell().Add(new Paragraph(col)
                        .SetFont(titleFont).SetFontSize(10)
                        .SetFontColor(ColorConstants.WHITE))
                    .SetBackgroundColor(new DeviceRgb(26, 86, 219))
                    .SetPadding(6));
            }

            // Rows
            for (int i = 0; i < Math.Min(candidates.Count, 100); i++)
            {
                var c = candidates[i];
                var bg = i % 2 == 0
                    ? new DeviceRgb(249, 250, 251)
                    : new DeviceRgb(239, 246, 255);

                var rowData = new[]
                {
                    $"{c.User.FirstName} {c.User.LastName}",
                    c.User.Email ?? "",
                    $"{c.TotalExperienceYears} yr(s)",
                    c.HighestEducation ?? "—",
                    c.CandidateSkills.Count.ToString(),
                    c.Applications.Count.ToString()
                };

                foreach (var val in rowData)
                {
                    table.AddCell(
                        new Cell().Add(
                            new Paragraph(val).SetFont(bodyFont).SetFontSize(9))
                        .SetBackgroundColor(bg).SetPadding(5));
                }
            }

            document.Add(table);
        }

        private async Task AddJobPdfSectionAsync(
            Document document, GenerateReportViewModel model,
            PdfFont titleFont, PdfFont bodyFont)
        {
            document.Add(new Paragraph("Job Report")
                .SetFont(titleFont).SetFontSize(16)
                .SetFontColor(new DeviceRgb(5, 150, 105)));

            var jobs = await _context.Jobs
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            document.Add(new Paragraph(
                $"Total Jobs: {jobs.Count} · " +
                $"Active: {jobs.Count(j => j.Status == JobStatus.Active)}")
                .SetFont(bodyFont).SetFontSize(11)
                .SetFontColor(new DeviceRgb(107, 114, 128)));

            document.Add(new Paragraph("\n"));

            float[] widths = { 2f, 2f, 1.5f, 1.5f, 1f, 1.5f, 1.5f };
            var table = new Table(UnitValue.CreatePercentArray(widths))
                .UseAllAvailableWidth();

            foreach (var h in new[]
                { "Title", "Company", "Type", "Status", "Apps",
                  "Shortlisted", "Posted" })
            {
                table.AddHeaderCell(
                    new Cell().Add(new Paragraph(h)
                        .SetFont(titleFont).SetFontSize(10)
                        .SetFontColor(ColorConstants.WHITE))
                    .SetBackgroundColor(new DeviceRgb(5, 150, 105))
                    .SetPadding(6));
            }

            for (int i = 0; i < Math.Min(jobs.Count, 100); i++)
            {
                var j = jobs[i];
                var bg = i % 2 == 0
                    ? new DeviceRgb(249, 250, 251)
                    : new DeviceRgb(240, 253, 244);

                foreach (var val in new[]
                {
                    j.Title, j.Company, j.JobType.ToString(),
                    j.Status.ToString(),
                    j.Applications.Count.ToString(),
                    j.Applications.Count(a => a.Status == ApplicationStatus.Shortlisted).ToString(),
                    j.PostedDate.ToString("yyyy-MM-dd")
                })
                {
                    table.AddCell(
                        new Cell().Add(
                            new Paragraph(val).SetFont(bodyFont).SetFontSize(9))
                        .SetBackgroundColor(bg).SetPadding(5));
                }
            }

            document.Add(table);
        }

        private async Task AddRankingPdfSectionAsync(
            Document document, GenerateReportViewModel model,
            PdfFont titleFont, PdfFont bodyFont)
        {
            document.Add(new Paragraph("AI Ranking Report")
                .SetFont(titleFont).SetFontSize(16)
                .SetFontColor(new DeviceRgb(217, 119, 6)));

            var appsQuery = _context.Applications
                .Include(a => a.Job)
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Where(a => a.AIMatchScore.HasValue);

            if (model.JobId.HasValue)
                appsQuery = appsQuery.Where(a => a.JobId == model.JobId.Value);

            if (model.MinScoreFilter.HasValue)
                appsQuery = appsQuery
                    .Where(a => a.AIMatchScore >= model.MinScoreFilter.Value);

            var applications = await appsQuery
                .OrderByDescending(a => a.AIMatchScore)
                .Take(200)
                .ToListAsync();

            document.Add(new Paragraph(
                $"Total Ranked: {applications.Count} · " +
                $"Avg Score: {(applications.Any() ? applications.Average(a => a.AIMatchScore ?? 0).ToString("F1") : "N/A")}%")
                .SetFont(bodyFont).SetFontSize(11)
                .SetFontColor(new DeviceRgb(107, 114, 128)));

            document.Add(new Paragraph("\n"));

            float[] widths = { 0.8f, 2f, 2.5f, 1.5f, 1.5f, 1.5f };
            var table = new Table(UnitValue.CreatePercentArray(widths))
                .UseAllAvailableWidth();

            foreach (var h in new[]
                { "Rank", "Candidate", "Job", "AI Score", "Skill %", "Status" })
            {
                table.AddHeaderCell(
                    new Cell().Add(new Paragraph(h)
                        .SetFont(titleFont).SetFontSize(10)
                        .SetFontColor(ColorConstants.WHITE))
                    .SetBackgroundColor(new DeviceRgb(217, 119, 6))
                    .SetPadding(6));
            }

            for (int i = 0; i < applications.Count; i++)
            {
                var a = applications[i];
                var score = a.AIMatchScore ?? 0;

                DeviceRgb bg;
                if (score >= 75) bg = new DeviceRgb(209, 250, 229);
                else if (score >= 50) bg = new DeviceRgb(219, 234, 254);
                else if (score >= 30) bg = new DeviceRgb(254, 249, 195);
                else bg = new DeviceRgb(254, 226, 226);

                foreach (var (val, bold) in new[]
                {
                    ((i + 1).ToString(), true),
                    ($"{a.Candidate.User.FirstName} {a.Candidate.User.LastName}", false),
                    (a.Job?.Title ?? "", false),
                    ($"{score:F1}%", true),
                    ($"{a.SkillMatchPercentage:F1}%", false),
                    (a.Status.ToString(), false)
                })
                {
                    var para = new Paragraph(val)
                        .SetFont(bold ? titleFont : bodyFont)
                        .SetFontSize(9);
                    table.AddCell(
                        new Cell().Add(para)
                        .SetBackgroundColor(bg).SetPadding(5));
                }
            }

            document.Add(table);
        }

        // ─── Delete & File Utilities ───────────────────────────────────────

        public async Task<(bool Success, string Error)> DeleteReportAsync(
            int reportId, string userId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null) return (false, "Report not found.");
            if (report.GeneratedByUserId != userId)
                return (false, "Access denied.");

            if (!string.IsNullOrEmpty(report.FilePath))
            {
                var fullPath = Path.Combine(
                    _env.WebRootPath,
                    report.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public byte[]? GetReportBytes(string filePath)
        {
            var fullPath = Path.Combine(
                _env.WebRootPath,
                filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : null;
        }

        public string GetContentType(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}