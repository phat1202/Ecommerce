namespace Ecommerce.Extensions.EmailSending
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
