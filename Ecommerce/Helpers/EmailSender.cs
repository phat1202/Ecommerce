using System.Net;
using System.Net.Mail;
using System.Net.WebSockets;

namespace Ecommerce.Helpers
{
    public class EmailSender
    {
        public void SendEmail(string email, string subject, string body, bool html)
        {
            var sender = "trg.tanphat@gmail.com";
            var pass = "cssqoxvjcgocjqem";
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential(sender, pass);
                    client.EnableSsl = true;
                    
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(sender);
                    message.To.Add(email);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = html;
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
