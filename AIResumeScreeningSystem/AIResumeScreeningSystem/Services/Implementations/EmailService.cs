using AIResumeScreeningSystem.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"]!;
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]!);
                var senderEmail = _configuration["EmailSettings:SenderEmail"]!;
                var senderName = _configuration["EmailSettings:SenderName"]!;
                var username = _configuration["EmailSettings:Username"]!;
                var password = _configuration["EmailSettings:Password"]!;

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(username, password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent to {Email} with subject: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                // Don't rethrow — email failure should not break app flow
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string firstName, string role)
        {
            var subject = "Welcome to AI Resume Screening System";
            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;'>
                    <div style='background:#1a56db;padding:20px;text-align:center;border-radius:8px 8px 0 0;'>
                        <h1 style='color:white;margin:0;'>AI Resume Screening</h1>
                    </div>
                    <div style='background:#f9fafb;padding:30px;border-radius:0 0 8px 8px;border:1px solid #e5e7eb;'>
                        <h2 style='color:#111827;'>Welcome, {firstName}!</h2>
                        <p style='color:#374151;'>Your account has been successfully created as a <strong>{role}</strong>.</p>
                        <p style='color:#374151;'>You can now log in and start using the platform.</p>
                        <div style='text-align:center;margin:30px 0;'>
                            <a href='#' style='background:#1a56db;color:white;padding:12px 30px;border-radius:6px;text-decoration:none;font-weight:bold;'>
                                Go to Dashboard
                            </a>
                        </div>
                        <p style='color:#6b7280;font-size:12px;'>If you did not create this account, please ignore this email.</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink)
        {
            var subject = "Reset Your Password - AI Resume Screening";
            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;'>
                    <div style='background:#1a56db;padding:20px;text-align:center;border-radius:8px 8px 0 0;'>
                        <h1 style='color:white;margin:0;'>Password Reset</h1>
                    </div>
                    <div style='background:#f9fafb;padding:30px;border-radius:0 0 8px 8px;border:1px solid #e5e7eb;'>
                        <h2 style='color:#111827;'>Hello, {firstName}</h2>
                        <p style='color:#374151;'>We received a request to reset your password. Click the button below:</p>
                        <div style='text-align:center;margin:30px 0;'>
                            <a href='{resetLink}' style='background:#dc2626;color:white;padding:12px 30px;border-radius:6px;text-decoration:none;font-weight:bold;'>
                                Reset Password
                            </a>
                        </div>
                        <p style='color:#6b7280;'>This link will expire in 24 hours.</p>
                        <p style='color:#6b7280;font-size:12px;'>If you did not request a password reset, please ignore this email.</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}