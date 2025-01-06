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

    public class RentalDto
    {
        public int Id { get; set; } // Zwracaj moje id
        public int CarId { get; set; } // Zwracaj moje id
        public int UserId { get; set; } // Zwracaj externalId
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public String StartLocation { get; set; }
        public String EndLocation { get; set; }
    }

    public class RentalToFront
    {
        public int id { get; set; }
        public Car Car { get; set; } 
        public int userId { get; set; }
        public string startDate { get; set; } 
        public string endDate { get; set; } 
        public decimal totalPrice { get; set; }   
        public string status { get; set; }  
        public string startLocation { get; set; }
        public string endLocation { get; set; }
        public RentalToFront(RentalDto data)
        {
            id = data.Id;
            //carId = data.CarId;
            userId = data.UserId;
            startDate = data.StartDate.ToShortDateString();
            endDate = data.EndDate.ToShortDateString();
            totalPrice = data.TotalPrice;
            status = data.Status.ToString();
            startLocation = data.StartLocation;
            endLocation = data.EndLocation;
        }
    }

    
}
