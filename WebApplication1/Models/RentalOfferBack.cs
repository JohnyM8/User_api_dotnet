namespace WebApplication1.Models
{
    public class RentalOfferBack
    {
        public int Id { get; set; }
        public CarDto? Car { get; set; }
        public decimal DailyRate { get; set; }
        public decimal InsuranceRate { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
