using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;

namespace WebApplication1.Models
{
    public class EmailRequest
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
    public static class EmailSender
    {
        public static bool SendReturnStartEmail(string email)
        {
            var emailR = new EmailRequest()
            {
                To = email,
                Subject = "Return proccess started",
                Body = "Your return proccess has been started!\n",
            };
            return SendEmail(emailR);
        }

        public static bool SendRentEmail(string email)
        {
            var emailR = new EmailRequest()
            {
                To = email,
                Subject = "New Rent",
                Body = "Your new car has been rent for you!\n",
            };
            return SendEmail(emailR);
        }
        public static bool SendOfferEmail(string email)
        {
            var emailR = new EmailRequest()
            {
                To = email,
                Subject = "New Offer",
                Body = "Your new offer has been sent to you!\n",
            };
            return SendEmail(emailR);
        }
        public static bool SendEmail(EmailRequest emailRequest)
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
                throw new Exception(ex.Message);
            }
        }
    }
}
