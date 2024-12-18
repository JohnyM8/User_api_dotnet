namespace WebApplication1.Models
{
    public class RentalOfferFront
    {
        public Car? car { get; set; }
        public User? user { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
    }
}
