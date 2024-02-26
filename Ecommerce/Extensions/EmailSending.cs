using System.Net;
using System.Net.Mail;

namespace Ecommerce.Extensions
{
    public class EmailSending
    {
        private string _hotmailSend;
        private string _hotmailPassword;

        public EmailSending(string hotmailSend, string hotmailPassword)
        {
            _hotmailSend = hotmailSend;
            _hotmailPassword = hotmailPassword;
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.live.com", 587))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential(_hotmailSend, _hotmailPassword);
                    client.EnableSsl = true;

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(_hotmailSend);
                    message.To.Add(recipient);
                    message.Subject = subject;
                    message.Body = body;

                    client.Send(message);
                    Console.WriteLine("Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
