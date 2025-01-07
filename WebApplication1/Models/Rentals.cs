using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Rental
    {
        [Key]
        public int id { get; set; } 
        public int carId { get; set; }  
        public int userId { get; set; }  
        public DateTime startDate { get; set; } 
        public DateTime endDate { get; set; }  
        public decimal totalPrice { get; set; }
        public RentalStatus status { get; set; } = RentalStatus.planned; 
        public string startLocation { get; set; } = "Plac Politechnik, Warszawa";
        public string endLocation { get; set; } = "Plac Politechniki, Warszawa";

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
