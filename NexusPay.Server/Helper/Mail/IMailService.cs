namespace NexusPay.Server.Helper.Mail
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
