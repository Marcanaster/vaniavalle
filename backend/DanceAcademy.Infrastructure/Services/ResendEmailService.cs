using DanceAcademy.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resend;

namespace DanceAcademy.Infrastructure.Services;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(IResend resend, IConfiguration configuration, ILogger<ResendEmailService> logger)
    {
        _resend = resend;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var fromEmail = _configuration["Resend:FromEmail"];
            var fromName = _configuration["Resend:FromName"];

            if (string.IsNullOrEmpty(fromEmail))
            {
                throw new InvalidOperationException("Resend:FromEmail is not configured.");
            }

            var message = new EmailMessage
            {
                From = string.IsNullOrEmpty(fromName) ? fromEmail : $"{fromName} <{fromEmail}>",
                To = to,
                Subject = subject,
                HtmlBody = htmlBody
            };

            var response = await _resend.EmailSendAsync(message);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} via Resend", to);
            throw;
        }
    }
}
