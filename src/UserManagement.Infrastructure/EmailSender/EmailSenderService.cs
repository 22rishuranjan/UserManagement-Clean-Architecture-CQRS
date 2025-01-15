


using System.Net.Mail;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Infrastructure.EmailSender;

public class EmailSenderService : IEmailSender
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient("localhost");
        
            client.Port = 25; // Default Papercut port
            client.EnableSsl = false; // No SSL for local testing with Papercut

            var mailMessage = new MailMessage
            {
                From = new MailAddress("noreply@yourdomain.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(to);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent to {to} successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                throw;
            }
        
    }
}