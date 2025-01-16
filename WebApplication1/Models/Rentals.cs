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
        public int id { get; set; } // Zwracaj moje id
        public int carId { get; set; } // Zwracaj moje id
        public int userId { get; set; } // Zwracaj externalId
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public decimal totalPrice { get; set; }
        public RentalStatus status { get; set; }
        public String startLocation { get; set; }
        public String endLocation { get; set; }
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
            id = data.id;
            //carId = data.CarId;
            userId = data.userId;
            startDate = data.startDate.ToShortDateString();
            endDate = data.endDate.ToShortDateString();
            totalPrice = data.totalPrice;
            status = data.status.ToString();
            startLocation = $"{data.startLocation}";
            endLocation = $"{data.endLocation}";
        }
    }

    
}
