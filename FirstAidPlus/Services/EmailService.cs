using System.Net;
using System.Net.Mail;

namespace FirstAidPlus.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");
            
            var smtpClient = new SmtpClient(smtpSettings["MailServer"])
            {
                Port = int.Parse(smtpSettings["MailPort"]),
                Credentials = new NetworkCredential(smtpSettings["SenderEmail"], smtpSettings["SenderPassword"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EmailService] Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error sending email to {toEmail}: {ex.Message}");
                // Re-throw or handle? For now re-throw to let controller know
                throw;
            }
        }
    }
}
