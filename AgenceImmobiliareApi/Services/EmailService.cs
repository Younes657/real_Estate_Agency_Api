using System.Net.Mail;
using System.Net;

namespace AgenceImmobiliareApi.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587; // or 465 for SSL
        private readonly string _smtpUser = "y_bouzenacha@estin.dz";
        private readonly string _smtpPass = "zqolhiqirexmkohv";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(_smtpUser, "NariImmobilier"); // Replace "Your Name" with the desired sender name
            var toAddress = new MailAddress(toEmail);

            using (var smtp = new SmtpClient
            {
                Host = _smtpServer,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass)
            })
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true, // Set to false if the body is plain text
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}
