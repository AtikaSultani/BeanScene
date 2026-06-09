using Microsoft.AspNetCore.Identity.UI.Services;

namespace BeanScene.Services
{
    public class IdentityEmailSender : IEmailSender
    {
        private readonly EmailSender _emailSender;

        public IdentityEmailSender(EmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            await _emailSender.SendEmailAsync(
                email,
                subject,
                htmlMessage);
        }
    }
}