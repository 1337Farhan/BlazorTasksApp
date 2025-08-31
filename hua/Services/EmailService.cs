using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace hua.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendVerificationEmailAsync(string to, string verificationToken);
    }

    public class EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger) : IEmailService
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // Simulate async operation
            await Task.Delay(100);

            // Log to console with pretty formatting
            _logger.LogInformation("""
                📧 EMAIL WOULD BE SENT:
                ┌─────────────────────────────────────────────────────
                │ To: {To}
                │ From: {From} ({DisplayName})
                │ Subject: {Subject}
                │ 
                │ Body:
                │ {Body}
                └─────────────────────────────────────────────────────
                """,
                to,
                _emailSettings.Email,
                _emailSettings.DisplayName,
                subject,
                body);
        }

        public async Task SendVerificationEmailAsync(string to, string verificationToken)
        {
            var subject = "Verify your email address";
            var verificationUrl = $"{_emailSettings.BaseUrl}/activate-user/{verificationToken}";

            var body = $@"
                <h2>Email Verification</h2>
                <p>Thank you for registering! Please click the link below to verify your email address:</p>
                <p><a href='{verificationUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email</a></p>
                <p>If you didn't request this, please ignore this email.</p>
                <p>This link will expire in 24 hours.</p>
            ";

            await SendEmailAsync(to, subject, body);
        }
        
        public async Task SendPasswordResetEmailAsync(string to, string passwordResetToken)
        {
            var subject = "Reset your password";
            var resetUrl = $"{_emailSettings.BaseUrl}/password-reset/{passwordResetToken}";
            
            var body = $@"
                <h2>Passowrd Reset</h2>
                <p>Please click the link below to reset your password</p>
                <p><a href='{resetUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                <p>If you didn't request this, please ignore this email.</p>
                <p>This link will expire in 24 hours.</p>
            ";

            await SendEmailAsync(to, subject, body);
        }
    }

    public class EmailSettings
    {
        public string Email { get; set; } = "noreply@yourapp.com";
        public string DisplayName { get; set; } = "Your App";
        public string BaseUrl { get; set; } = "https://localhost:7192";
    }
}