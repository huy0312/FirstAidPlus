using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FirstAidPlus.Services
{
    public class MockEmailService : IEmailService
    {
        private readonly IWebHostEnvironment _env;

        public MockEmailService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var logPath = Path.Combine(_env.ContentRootPath, "logs", "emails.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            var logEntry = $"[{DateTime.Now}] TO: {toEmail}\nSUBJECT: {subject}\nMESSAGE:\n{message}\n---------------------------------------------------\n\n";

            await File.AppendAllTextAsync(logPath, logEntry);
        }
    }
}
