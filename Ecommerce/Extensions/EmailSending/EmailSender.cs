using System.Net;
using System.Net.Mail;
using System.Net.WebSockets;

namespace Ecommerce.Extensions.EmailSending
{
    public class EmailSender
    {
        public void SendEmail(string email, string subject, string body)
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
                    
                    client.Send(message);
                    Console.WriteLine("Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    var sender = "josiahewalshu@hotmail.com";
        //    var password = "jjosi123";
        //    var client = new SmtpClient("smtp.office365.com", 587)
        //    {
        //        EnableSsl = true,
        //        Credentials = new NetworkCredential(sender, password),
        //    };
        //    return client.SendMailAsync(
        //        new MailMessage(from: sender,
        //        to: email,
        //        subject, message));
        //}
    }
}
