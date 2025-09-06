using System.Net.Mail;
using System.Net;
using Librelia.Models;
using Microsoft.Extensions.Options;

namespace Librelia.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;
        // Le configurazioni sono da inserire in "appsetting.json"
        private readonly IOptions<EmailSettings> _settings;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _logger = logger;
            _settings = settings;
            _smtpUsername = settings.Value.Username;
            _smtpPassword = settings.Value.Password;
        }

        private static readonly SemaphoreSlim _emailSemaphore = new SemaphoreSlim(1, 1);

        public async Task SendMailAsync(string recipientAddress, string subject, string body)
        {
            await _emailSemaphore.WaitAsync();
            try
            {
                using (var smtpClient = new SmtpClient(_settings.Value.SmtpServer, _settings.Value.Port))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage("noreply@librelia.com", recipientAddress, subject, body);

                    mailMessage.IsBodyHtml = true;
                    
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore nell'invio dell'email: \n" + ex + "\n");
                
            }
            finally
            {
                _emailSemaphore.Release();
            }
        }

    }
}