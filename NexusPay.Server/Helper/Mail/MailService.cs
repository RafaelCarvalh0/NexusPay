using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NexusPay.Server.Options;

namespace NexusPay.Server.Helper.Mail
{
    public class MailService : IMailService
    {
        private readonly SmtpOptions _smtpOptions;

        public MailService(IOptions<SmtpOptions> smtpOptions)
        {
            _smtpOptions = smtpOptions.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpOptions.FromName, _smtpOptions.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_smtpOptions.FromEmail, _smtpOptions.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
