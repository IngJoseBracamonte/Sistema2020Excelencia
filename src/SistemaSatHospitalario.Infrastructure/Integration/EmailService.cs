using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Infrastructure.Integration
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("SatHospitalario System", _config["EmailSettings:SenderEmail"] ?? "noreply@sathospital.com"));
                message.To.Add(new MailboxAddress("System Admin", to));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                var host = _config["EmailSettings:SmtpHost"];
                var portStr = _config["EmailSettings:SmtpPort"];
                var user = _config["EmailSettings:SmtpUser"];
                var pass = _config["EmailSettings:SmtpPass"];

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    _logger.LogWarning("EmailService: SMTP not configured. Skipping email send.");
                    return;
                }

                int port = int.TryParse(portStr, out var p) ? p : 2525;

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTlsWhenAvailable);
                await client.AuthenticateAsync(user, pass);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}", to);
                // No re-lanzamos la excepción porque esto corre en background (Fire and Forget) general
            }
        }
    }
}
