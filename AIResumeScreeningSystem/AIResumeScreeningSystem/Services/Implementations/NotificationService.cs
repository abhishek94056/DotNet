using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Notification;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepo,
            IEmailService emailService,
            AppDbContext context,
            ILogger<NotificationService> logger)
        {
            _notificationRepo = notificationRepo;
            _emailService = emailService;
            _context = context;
            _logger = logger;
        }

        // ─── Read Notifications ────────────────────────────────────────────

        public async Task<NotificationListViewModel> GetUserNotificationsAsync(
            string userId, bool unreadOnly = false)
        {
            var notifications = await _notificationRepo
                .GetUserNotificationsAsync(userId, unreadOnly);

            return new NotificationListViewModel
            {
                Notifications = notifications.Select(MapToViewModel).ToList(),
                UnreadCount = notifications.Count(n => !n.IsRead),
                TotalCount = notifications.Count,
                ShowOnlyUnread = unreadOnly
            };
        }

        public async Task<int> GetUnreadCountAsync(string userId)
            => await _notificationRepo.GetUnreadCountAsync(userId);

        // ─── Create / Manage ───────────────────────────────────────────────

        public async Task CreateNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type = NotificationType.Info,
            string? actionUrl = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    ActionUrl = actionUrl
                };

                await _notificationRepo.AddAsync(notification);
                await _notificationRepo.SaveChangesAsync();

                // Keep inbox clean
                await _notificationRepo.DeleteOldNotificationsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating notification for user {UserId}", userId);
            }
        }

        public async Task MarkAsReadAsync(int notificationId)
            => await _notificationRepo.MarkAsReadAsync(notificationId);

        public async Task MarkAllAsReadAsync(string userId)
            => await _notificationRepo.MarkAllAsReadAsync(userId);

        public async Task DeleteOldNotificationsAsync(string userId)
            => await _notificationRepo.DeleteOldNotificationsAsync(userId);

        // ─── Domain Notifications ──────────────────────────────────────────

        public async Task NotifyApplicationSubmittedAsync(int applicationId)
        {
            try
            {
                var application = await _context.Applications
                    .Include(a => a.Job)
                    .Include(a => a.Candidate).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(a => a.Id == applicationId);

                if (application == null) return;

                var candidate = application.Candidate;
                var job = application.Job;
                var candidateUser = candidate.User;

                // Notify candidate
                await CreateNotificationAsync(
                    candidateUser.Id,
                    "Application Submitted",
                    $"Your application for {job?.Title} at {job?.Company} has been submitted.",
                    NotificationType.Success,
                    $"/Application/Details/{applicationId}");

                // Notify recruiter
                var recruiter = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == job!.PostedByUserId);
                if (recruiter != null)
                {
                    await CreateNotificationAsync(
                        recruiter.Id,
                        "New Application Received",
                        $"{candidateUser.FirstName} {candidateUser.LastName} applied for {job?.Title}.",
                        NotificationType.Info,
                        $"/Application/Index?jobId={job?.Id}");
                }

                // Email candidate
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            candidateUser.Email!,
                            $"Application Submitted — {job?.Title}",
                            BuildApplicationSubmittedEmail(
                                candidateUser.FirstName,
                                job?.Title ?? "N/A",
                                job?.Company ?? "N/A"));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send application email");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending application submitted notification for {AppId}", applicationId);
            }
        }

        public async Task NotifyApplicationStatusChangedAsync(
            int applicationId, string newStatus)
        {
            try
            {
                var application = await _context.Applications
                    .Include(a => a.Job)
                    .Include(a => a.Candidate).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(a => a.Id == applicationId);

                if (application == null) return;

                var candidateUser = application.Candidate.User;
                var job = application.Job;
                var type = newStatus.ToLower() switch
                {
                    "approved" => NotificationType.Success,
                    "rejected" => NotificationType.Error,
                    "shortlisted" => NotificationType.Warning,
                    _ => NotificationType.Info
                };

                await CreateNotificationAsync(
                    candidateUser.Id,
                    $"Application {newStatus}",
                    $"Your application for {job?.Title} at {job?.Company} has been {newStatus.ToLower()}.",
                    type,
                    $"/Application/Details/{applicationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending status change notification for {AppId}", applicationId);
            }
        }

        public async Task NotifyCandidateShortlistedAsync(int applicationId)
        {
            try
            {
                var application = await _context.Applications
                    .Include(a => a.Job)
                    .Include(a => a.Candidate).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(a => a.Id == applicationId);

                if (application == null) return;

                var candidateUser = application.Candidate.User;
                var job = application.Job;

                await CreateNotificationAsync(
                    candidateUser.Id,
                    "🎉 You've Been Shortlisted!",
                    $"Congratulations! You've been shortlisted for {job?.Title} at {job?.Company}.",
                    NotificationType.Success,
                    $"/Application/Details/{applicationId}");

                // Send email
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            candidateUser.Email!,
                            $"You've Been Shortlisted — {job?.Title}",
                            BuildShortlistedEmail(
                                candidateUser.FirstName,
                                job?.Title ?? "N/A",
                                job?.Company ?? "N/A"));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send shortlist email");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending shortlist notification for {AppId}", applicationId);
            }
        }

        public async Task NotifyCandidateRejectedAsync(int applicationId)
        {
            try
            {
                var application = await _context.Applications
                    .Include(a => a.Job)
                    .Include(a => a.Candidate).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(a => a.Id == applicationId);

                if (application == null) return;

                var candidateUser = application.Candidate.User;
                var job = application.Job;

                await CreateNotificationAsync(
                    candidateUser.Id,
                    "Application Update",
                    $"We regret to inform you that your application for {job?.Title} at {job?.Company} was not successful.",
                    NotificationType.Warning,
                    $"/Application/Details/{applicationId}");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            candidateUser.Email!,
                            $"Application Update — {job?.Title}",
                            BuildRejectedEmail(
                                candidateUser.FirstName,
                                job?.Title ?? "N/A",
                                job?.Company ?? "N/A"));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send rejection email");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending rejection notification for {AppId}", applicationId);
            }
        }

        public async Task NotifyNewJobPostedAsync(int jobId)
        {
            try
            {
                var job = await _context.Jobs
                    .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                    .FirstOrDefaultAsync(j => j.Id == jobId);
                if (job == null) return;

                var jobSkillNames = job.JobSkills
                    .Select(js => js.Skill.Name.ToLower()).ToHashSet();

                // Find candidates with matching skills
                var matchingCandidates = await _context.Candidates
                    .Include(c => c.User)
                    .Include(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
                    .Where(c => c.IsAvailable && c.User.IsActive &&
                                c.CandidateSkills.Any(cs =>
                                    jobSkillNames.Contains(cs.Skill.Name.ToLower())))
                    .Take(50)
                    .ToListAsync();

                foreach (var candidate in matchingCandidates)
                {
                    await CreateNotificationAsync(
                        candidate.UserId,
                        "New Job Match Found",
                        $"A new job matching your skills: {job.Title} at {job.Company}.",
                        NotificationType.Info,
                        $"/Job/Details/{jobId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending new job notification for Job {JobId}", jobId);
            }
        }

        public async Task NotifyResumeParseCompleteAsync(int resumeId)
        {
            try
            {
                var resume = await _context.Resumes
                    .Include(r => r.Candidate).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(r => r.Id == resumeId);
                if (resume == null) return;

                var skillCount = resume.ParsedSkills?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries).Length ?? 0;

                var msg = resume.Status == ResumeStatus.Parsed
                    ? $"Your resume '{resume.FileName}' has been parsed. {skillCount} skill(s) extracted."
                    : $"Resume parsing failed for '{resume.FileName}'. Please try re-uploading.";

                await CreateNotificationAsync(
                    resume.Candidate.UserId,
                    resume.Status == ResumeStatus.Parsed
                        ? "Resume Parsed Successfully"
                        : "Resume Parse Failed",
                    msg,
                    resume.Status == ResumeStatus.Parsed
                        ? NotificationType.Success : NotificationType.Error,
                    $"/Resume/Details/{resumeId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending resume parse notification for {ResumeId}", resumeId);
            }
        }

        // ─── Email Templates ───────────────────────────────────────────────

        private static string BuildApplicationSubmittedEmail(
            string firstName, string jobTitle, string company) => $@"
<div style='font-family:Inter,Arial,sans-serif;max-width:600px;margin:auto;'>
    <div style='background:linear-gradient(135deg,#1e3a5f,#1a56db);
                padding:30px;text-align:center;border-radius:12px 12px 0 0;'>
        <h1 style='color:white;margin:0;font-size:24px;'>Application Submitted ✅</h1>
    </div>
    <div style='background:#f9fafb;padding:30px;border-radius:0 0 12px 12px;
                border:1px solid #e5e7eb;'>
        <p style='color:#374151;font-size:16px;'>Hi <strong>{firstName}</strong>,</p>
        <p style='color:#374151;'>Your application for
            <strong>{jobTitle}</strong> at <strong>{company}</strong>
            has been successfully submitted.</p>
        <div style='background:#dbeafe;border-radius:8px;padding:16px;margin:20px 0;'>
            <p style='color:#1e40af;margin:0;font-weight:600;'>What happens next?</p>
            <ul style='color:#1e40af;margin:8px 0 0;'>
                <li>Our AI will calculate your match score</li>
                <li>Recruiter will review your application</li>
                <li>You'll be notified of any status updates</li>
            </ul>
        </div>
        <p style='color:#6b7280;font-size:13px;'>
            Best of luck! The AI Resume Screening Team
        </p>
    </div>
</div>";

        private static string BuildShortlistedEmail(
            string firstName, string jobTitle, string company) => $@"
<div style='font-family:Inter,Arial,sans-serif;max-width:600px;margin:auto;'>
    <div style='background:linear-gradient(135deg,#065f46,#059669);
                padding:30px;text-align:center;border-radius:12px 12px 0 0;'>
        <h1 style='color:white;margin:0;font-size:24px;'>You're Shortlisted! 🎉</h1>
    </div>
    <div style='background:#f9fafb;padding:30px;border-radius:0 0 12px 12px;
                border:1px solid #e5e7eb;'>
        <p style='color:#374151;font-size:16px;'>Hi <strong>{firstName}</strong>,</p>
        <p style='color:#374151;'>
            Congratulations! You've been shortlisted for
            <strong>{jobTitle}</strong> at <strong>{company}</strong>.
        </p>
        <div style='background:#d1fae5;border-radius:8px;padding:16px;margin:20px 0;'>
            <p style='color:#065f46;margin:0;'>
                🚀 The recruiter has reviewed your profile and selected you as a top candidate.
                Expect to hear about next steps soon!
            </p>
        </div>
        <p style='color:#6b7280;font-size:13px;'>Good luck! The AI Resume Screening Team</p>
    </div>
</div>";

        private static string BuildRejectedEmail(
            string firstName, string jobTitle, string company) => $@"
<div style='font-family:Inter,Arial,sans-serif;max-width:600px;margin:auto;'>
    <div style='background:linear-gradient(135deg,#7c3aed,#6d28d9);
                padding:30px;text-align:center;border-radius:12px 12px 0 0;'>
        <h1 style='color:white;margin:0;font-size:24px;'>Application Update</h1>
    </div>
    <div style='background:#f9fafb;padding:30px;border-radius:0 0 12px 12px;
                border:1px solid #e5e7eb;'>
        <p style='color:#374151;font-size:16px;'>Hi <strong>{firstName}</strong>,</p>
        <p style='color:#374151;'>
            Thank you for applying to <strong>{jobTitle}</strong> at <strong>{company}</strong>.
            After careful consideration, we've decided to move forward with other candidates.
        </p>
        <div style='background:#f3f4f6;border-radius:8px;padding:16px;margin:20px 0;'>
            <p style='color:#374151;margin:0;'>
                💡 Tip: Upload an updated resume and continue improving your skills
                to boost your AI match score for future applications.
            </p>
        </div>
        <p style='color:#6b7280;font-size:13px;'>
            Keep going! The AI Resume Screening Team
        </p>
    </div>
</div>";

        // ─── Mapper ────────────────────────────────────────────────────────

        private static NotificationViewModel MapToViewModel(Notification n) => new()
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt,
            ActionUrl = n.ActionUrl
        };
    }
}