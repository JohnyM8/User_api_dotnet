using Org.BouncyCastle.Bcpg;

namespace WebApplication1.Models
{
    public class RentedCarsRequest
    {
        public int UserId { get; set; }
    }

    public class RentedCarsRequestDto
    {
        public int CustomerId { get; set; }
        public string RentalName { get; set; } = Constants.RentalName;

        public RentedCarsRequestDto() { }

        public RentedCarsRequestDto(RentedCarsRequest data)
        {
            CustomerId = data.UserId;
        }
    }
}
