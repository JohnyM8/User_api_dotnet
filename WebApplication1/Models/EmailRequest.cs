using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
//using System.Net.Mail;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IO;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using System.Net;

namespace WebApplication1.Models
{
    public class EmailRequestOld
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }

    public class EmailRequest
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public MimeEntity? Body { get; set; }
    }
    public static class EmailSender
    {
        public static bool SendReturnEndEmail(string email , ReturnConfReq data)
        {
            var emailR = new EmailRequest()
            {
                To = email,
                Subject = "Your return has been accepted!\n",
                Body = new TextPart("plain")
                {
                    Text = 
                    $"Employee notes: {data.EmployeeNotes}\n" +
                    $"Date of return: {data.ReturnDate}"
                },
            };
            return SendEmail(emailR);
        }
        public static bool SendReturnStartEmail(string email , ReturnRecordDto data)
        {
            var emailR = new EmailRequest()
            {
                To = email,
                Subject = "Return proccess started",
                Body = new TextPart("plain")
                { 
                    Text = "Your return proccess has been started!\n"
                },
            };
            return SendEmail(emailR);
        }

        public static bool SendRentEmail(string email , RentalDto rental , Car car)
        {
            var emailR = new EmailRequest()
            {
                To = email,
                Subject = "New Rent",
                Body = new TextPart("plain")
                {
                    Text = "Your new car has been rented for you!\n\n" +
                    $"Car model: {car.model}\n" +
                    $"Producer: {car.producer}\n" +
                    $"Type: {car.type}\n\n" +
                    $"Rentals details:\n" +
                    $"Start date: {rental.startDate.DateOnly()}\n" +
                    $"End date: {rental.endDate.DateOnly()}\n" +
                    $"Start location: {rental.startLocation}\n" +
                    $"End location: {rental.endLocation}\n"
        },
            };
            return SendEmail(emailR);
        }
        //public static bool SendOfferEmail2(string email, RentalOfferDto rental)
        //{
        //    var emailR = new EmailRequest()
        //    {
        //        To = email,
        //        Subject = "New Offer",
        //    };

        //    (string path, string filename) = FileCreator.CreateRentalOfferFile(rental);

        //    var body = new TextPart("plain")
        //    {
        //        Text = "Heres your's offer details.\n"
        //    };

        //    var stream = File.OpenRead(path);
        //    var attachment = new MimePart("document", "pdf")
        //    {
        //        Content = new MimeContent(stream),
        //        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
        //        ContentTransferEncoding = ContentEncoding.Base64,
        //        FileName = Path.GetFileName(filename)
        //    };



        //    var multipart = new Multipart("mixed");

        //    multipart.Add(body);
        //    multipart.Add(attachment);


        //    emailR.Body = multipart;

        //    //stream.Close();
        //    //File.Delete(path);

        //    bool response = SendEmail(emailR, path);

        //    stream.Close();
        //    File.Delete(path);

        //    return response;
        //}

        public static bool SendOfferEmail(string email , RentalOfferDto rental , OfferRequestDto offer)
        {
            var emailR = new EmailRequest()
            {
                To = email,
                Subject = "New Offer",
            };

            (string path , string filename) = FileCreator.CreateRentalOfferFile(rental , offer);

            var body = new TextPart("plain")
            {
                Text = "Your's offer details are in the attachment below.\n"
            };

            var stream = File.OpenRead(path);
            var attachment = new MimePart("document", "pdf")
            {
                Content = new MimeContent(stream),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(filename)
            };

            

            var multipart = new Multipart("mixed");

            multipart.Add(body);
            multipart.Add(attachment);

            
            emailR.Body = multipart;

            //stream.Close();
            //File.Delete(path);

            bool response = SendEmail(emailR);

            stream.Close();
            File.Delete(path);

            return response;
        }
        public static bool SendEmail(EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Subject))
            {
                return false;
            }

            try
            {
                MimeMessage message = new MimeMessage();

                message.From.Add(new MailboxAddress("Admin", "user.api.adnin@gmail.com"));

                message.To.Add(MailboxAddress.Parse(emailRequest.To));

                message.Subject = emailRequest.Subject;

                message.Body = emailRequest.Body;

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
