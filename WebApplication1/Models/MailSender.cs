using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Text.Json;
using MailKit.Net.Smtp;


namespace WebApplication1.Models
{
    public static class MailSender
    {
        public static bool SendMail(EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Subject) || string.IsNullOrEmpty(emailRequest.Body))
            {
                return false;
            }

            try
            {
                MimeMessage message = new MimeMessage();

                message.From.Add(new MailboxAddress("Admin", "user.api.adnin@gmail.com"));

                message.To.Add(MailboxAddress.Parse(emailRequest.To));

                message.Subject = emailRequest.Subject;

                message.Body = new TextPart("plain")
                {
                    Text = emailRequest.Body
                };

                using (var smtpClient = new SmtpClient()) // Podaj swój SMTP serwer
                {
                    smtpClient.CheckCertificateRevocation = false;
                    smtpClient.Connect("smtp.gmail.com", 465, true);

                    smtpClient.Authenticate("user.api.adnin@gmail.com", "wbrz wood sdcd ygib");

                    smtpClient.Send(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
