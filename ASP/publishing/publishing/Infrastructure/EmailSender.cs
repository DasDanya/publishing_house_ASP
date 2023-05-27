﻿using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace publishing.Infrastructure
{
    public class EmailSender
    {

        const string emailSender = "mail";
        const string emailPassword = "password";
        const string senderName = "company";
        const string smtpClient = "smtp.mail.ru";
        const int port = 2525;

        public void SendEmail(string email, string subject, string message)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(emailSender, senderName);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = $"<div>{message}</div>";

                using (SmtpClient client = new SmtpClient(smtpClient))
                {
                    client.Credentials = new NetworkCredential(emailSender, emailPassword);
                    client.Port = port;
                    client.EnableSsl = true;
                    client.Send(mailMessage);
                    mailMessage.Dispose();
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void SendEmailWithDocument(string pathToFile, string email, string subject, string message)
        {
            try
            {

                //SmtpClient client = new SmtpClient("smtp.mail.ru");
                //client.Credentials = new NetworkCredential("klimachkov.danilka@mail.ru", "jU2xx2TyG30Qz9RpT9UY");
                //client.Port = 2525;
                //client.EnableSsl = true;

                //using (var mailMessage = new MailMessage()) 
                //{
                //    mailMessage.IsBodyHtml = true;
                //    mailMessage.From = new MailAddress("klimachkov.danilka@mail.ru", "Издательство");
                //    mailMessage.To.Add(email);
                //    mailMessage.Subject = subject;
                //    mailMessage.Body = $"<div>{message}</div>";
                //    mailMessage.Attachments.Add(new Attachment(pathToFile));

                //    client.Send(mailMessage);
                //}

                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(emailSender, senderName);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = $"<div>{message}</div>";
                mailMessage.Attachments.Add(new Attachment(pathToFile));

                using (SmtpClient client = new SmtpClient(smtpClient))
                {
                    client.Credentials = new NetworkCredential(emailSender, emailPassword);
                    client.Port = port;
                    client.EnableSsl = true;
                    client.Send(mailMessage);
                    mailMessage.Dispose();
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}

