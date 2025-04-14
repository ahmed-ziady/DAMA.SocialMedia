using DAMA.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DAMA.Infrastructure.Services
{
    public class EmailService(IConfiguration config) : IEmailService
    {
        private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // ✅ Load Email Configuration
                string smtpServer = _config["Email:SmtpServer"] ?? throw new InvalidOperationException("SMTP Server is missing.");
                int smtpPort = int.Parse(_config["Email:Port"] ?? "587");
                bool enableSsl = bool.Parse(_config["Email:EnableSsl"] ?? "true");
                string email = _config["Email:Username"] ?? throw new InvalidOperationException("Email address is missing.");
                string password = _config["Email:Password"] ?? throw new InvalidOperationException("Email password is missing.");

                using var client = new SmtpClient(smtpServer, smtpPort);
                client.Credentials = new NetworkCredential(email, password);
                client.EnableSsl = enableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email, "DAMA Team"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                throw new InvalidOperationException($"SMTP Error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected Error: {ex.Message}", ex);
            }
        }
    }
}
