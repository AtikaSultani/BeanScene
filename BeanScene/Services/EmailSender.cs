using MailKit.Net.Smtp;
using MimeKit;

namespace BeanScene.Services
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlMessage)
        {
            var settings =
                _configuration.GetSection("Email");

            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    settings["FromName"],
                    settings["FromAddress"]));

            email.To.Add(
                MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            email.Body =
                new TextPart("html")
                {
                    Text = htmlMessage
                };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                settings["SmtpHost"],
                int.Parse(settings["SmtpPort"]),
                false);

            await smtp.AuthenticateAsync(
                settings["Username"],
                settings["Password"]);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}