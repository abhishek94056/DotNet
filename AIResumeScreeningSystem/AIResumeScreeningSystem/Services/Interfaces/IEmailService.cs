namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendWelcomeEmailAsync(string toEmail, string firstName, string role);
        Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink);
    }
}