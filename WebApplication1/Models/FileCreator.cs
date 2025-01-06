using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;


namespace WebApplication1.Models
{
    public static class FileCreator
    {
        
        public static (string , string) CreateRentalOfferFile(RentalOfferDto rental)
        {
            string DirectoryName = "TestFile";
            string filename = $"Offer{DateTime.Now.Day}_{DateTime.Now.Month}" +
                $"_{DateTime.Now.Year}_{DateTime.Now.Hour},{DateTime.Now.Minute}.pdf";

            string filePath = $"{DirectoryName}/{filename}";

            bool exists = System.IO.Directory.Exists(DirectoryName);

            if (!exists)
                System.IO.Directory.CreateDirectory(DirectoryName);

            string carModel = "None";
            string carProducer = "None";
            int yearOfProduction = 0;
            int numberOfSeats = 0;
            decimal dailyRate = 0m;
            decimal insuranceRate = 0m;
            decimal totalCost = 0;
            DateTime validUntil = DateTime.Now;
            string location = "none";
            bool isAvailable = false;

            if (rental != null)
            {
                dailyRate = rental.DailyRate;
                insuranceRate = rental.InsuranceRate;
                totalCost = rental.TotalCost;
                validUntil = rental.ValidUntil;
                if(rental.Car != null)
                {
                    carModel = rental.Car.model;
                    carProducer = rental.Car.producer;
                    yearOfProduction = rental.Car.yearOfProduction;
                    numberOfSeats = rental.Car.numberOfSeats;
                    location = rental.Car.location;
                    isAvailable = rental.Car.isAvailable;
                }
            }
            //PdfWriter writer = new PdfWriter(filePath);
            //PdfDocument pdf = new PdfDocument(writer);
            //Document document = new Document(pdf);
            //Paragraph header = new Paragraph("HEADER")
            //   .SetTextAlignment(TextAlignment.CENTER)
            //   .SetFontSize(20);

            //document.Add(header);
            //document.Close();
            // Tworzenie pliku PDF
            using (PdfWriter writer = new PdfWriter(filePath))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);

                    // Nagłówek
                    document.Add(new Paragraph("Rental Offer")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20)
                        //.SetBold()
                        .SetMarginBottom(20));


                    document.Add(new Paragraph($"Car Model: {carModel}"));
                    document.Add(new Paragraph($"Producer: {carProducer}"));
                    document.Add(new Paragraph($"Number of Seats: {numberOfSeats}"));
                    document.Add(new Paragraph($"Daily Rate: ${dailyRate:F2}"));
                    document.Add(new Paragraph($"Insurance Rate: ${insuranceRate:F2}"));
                    document.Add(new Paragraph($"Valid Until: {validUntil:dd-MM-yyyy}"));
                    document.Add(new Paragraph($"Location: {location}"));
                    document.Add(new Paragraph($"Available: {(isAvailable ? "Yes" : "No")}"));

                    // Dodanie podsumowania
                    document.Add(new Paragraph("\n--- Summary ---")
                        );
                    document.Add(new Paragraph($"Total Price for 1 day (incl. insurance): ${(dailyRate + insuranceRate):F2}")
                        .SetMarginBottom(20));

                    // Zamknięcie dokumentu
                    document.Close();
                }
            }


            return (filePath , filename);
        }
    }
}
