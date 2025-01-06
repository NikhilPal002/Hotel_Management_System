using MailKit.Net.Smtp;
using MimeKit;

 
 
 
namespace Hotel_Management.Services
{
    public class EmailService
    {
        private readonly string smtpServer = "smtp.gmail.com"; // Use Gmail's SMTP server or another SMTP provider
        private readonly int smtpPort = 587;
        private readonly string smtpUser = "nikhilpal1732@gmail.com";  // Your Gmail address
        private readonly string smtpPass = "hziv sgar jvhg idqv"; // Your Gmail password or App Password
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Hotel_Management", smtpUser));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
 
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body // Set email body (HTML format)
            };
 
            emailMessage.Body = bodyBuilder.ToMessageBody();
 
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}