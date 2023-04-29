using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace publishing.Infrastructure
{
    public class EmailSender
    {
        private readonly ILogger<EmailSender> _logger;


        public void SendEmail(string email, string subject, string message) 
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress("klimachkov.danilka@mail.ru", "Издательство");
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = $"<div>{message}</div>";

                using (SmtpClient client = new SmtpClient("smtp.mail.ru"))
                {
                    client.Credentials = new NetworkCredential("klimachkov.danilka@mail.ru", "jU2xx2TyG30Qz9RpT9UY");
                    client.Port = 2525;
                    client.EnableSsl = true;
                    client.Send(mailMessage);
                }
            }
            catch(Exception ex) 
            { 
            
            }
            
        }
    } 
}
