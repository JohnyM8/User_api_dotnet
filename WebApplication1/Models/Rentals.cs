using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Rental
    {
        [Key]
        public int id { get; set; }                    // Unique identifier for each rental
        public int carId { get; set; }                       // Foreign key reference to the Cars table

        public int userId { get; set; }                  // Foreign key reference to the Customers table

        public DateTime startDate { get; set; }              // The start date and time of the rental

        public DateTime endDate { get; set; }               // The end date and time of the rental (nullable if rental is in progress)

        public decimal totalPrice { get; set; }              // The total price for the rental
        public RentalStatus status { get; set; } = RentalStatus.planned;      // Rental status (default: Created)

        public string startLocation { get; set; } = "Plac Politechnik, Warszawa";           // The location where the car is picked up

        public string endLocation { get; set; } = "Plac Politechniki, Warszawa";          // The location where the car is dropped off (nullable)

        // public int? MileageAtStart { get; set; }             // Mileage of the car at the start of the rental

        // public int? MileageAtEnd { get; set; }               // Mileage of the car at the end of the rental (nullable if rental is in progress)

        //public ReturnRecord? ReturnRecord { get; set; }     // Return record associated with the rental (nullable)

    }

    public class RentalToFront
    {
        public int id { get; set; }                    // Unique identifier for each rental
        public int carId { get; set; }                       // Foreign key reference to the Cars table

        public int userId { get; set; }                  // Foreign key reference to the Customers table

        public string startDate { get; set; }              // The start date and time of the rental

        public string endDate { get; set; }               // The end date and time of the rental (nullable if rental is in progress)

        public decimal totalPrice { get; set; }              // The total price for the rental
        public string status { get; set; }      // Rental status (default: Created)

        public string startLocation { get; set; } = "Plac Politechnik, Warszawa";           // The location where the car is picked up

        public string endLocation { get; set; } = "Plac Politechniki, Warszawa";          // The location where the car is dropped off (nullable)

        // public int? MileageAtStart { get; set; }             // Mileage of the car at the start of the rental

        // public int? MileageAtEnd { get; set; }               // Mileage of the car at the end of the rental (nullable if rental is in progress)

        //public ReturnRecord? ReturnRecord { get; set; }     // Return record associated with the rental (nullable)

        public RentalToFront(Rental data)
        {
            id = data.id;
            carId = data.carId;
            userId = data.userId;
            startDate = data.startDate.ToShortDateString();
            endDate = data.endDate.ToShortDateString();
            totalPrice = data.totalPrice;
            status = data.status.ToString();
            startLocation = data.startLocation;
            endLocation = data.endLocation;
        }
    }
}
