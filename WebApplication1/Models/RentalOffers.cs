using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    //public class RentalOfferDto
    //{
    //    public int Id { get; set; }
    //    public CarDto? Car { get; set; }
    //    public decimal DailyRate { get; set; }
    //    public decimal InsuranceRate { get; set; }
    //    public decimal TotalCost { get; set; }
    //    public DateTime ValidUntil { get; set; }
    //}

    public class RentalOffer
    {
        [Key]
        public int id { get; set; }
        public int carId { get; set; }
        public int userId { get; set; }
        public decimal dailyRate { get; set; }
        public decimal insuranceRate { get; set; }
        public DateTime validUntil { get; set; }
        public bool isActive { get; set; }

        // Navigation property
    }

    public class RentalOfferDto
    {
        public int userId { get; set; }
        public int? carId { get; set; }
        public decimal dailyRate { get; set; }
        public decimal insuranceRate { get; set; }
        public DateTime validUntil { get; set; }
        public bool isActive { get; set; }
    }

    public class RentalOfferFront
    {
        public Car? car { get; set; }
        public User? user { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
    }
}
