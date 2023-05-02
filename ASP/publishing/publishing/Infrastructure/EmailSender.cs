using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace publishing.Infrastructure
{
    public class EmailSender
    {
        public void SendEmail(string email, string subject, string message)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress("your_mail", "name_company");
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = $"<div>{message}</div>";

                using (SmtpClient client = new SmtpClient("smtp.mail.ru")) // для майл почты
                {
                    client.Credentials = new NetworkCredential("your_mail", "your_password");
                    client.Port = 2525; // для майл почты
                    client.EnableSsl = true;
                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
