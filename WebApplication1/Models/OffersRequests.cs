namespace WebApplication1.Models
{
    public class OfferRequestDto
    {
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateOnly birthday { get; set; }
        public DateOnly driverLicenseReceiveDate { get; set; }
        public string RentalName { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }

        public OfferRequestDto() { }
        public OfferRequestDto(OfferRequestFront data)
        {
            CarId = data.CarId;
            CustomerId = data.CustomerId;
            PlannedStartDate = DateTime.Parse(data.PlannedStartDate);
            PlannedEndDate = DateTime.Parse(data.PlannedEndDate);
        }

        public void UpdateUserData(UserDto user)
        {
            firstName = user.firstname;
            lastName = user.lastname;
            birthday = user.birthday;
            driverLicenseReceiveDate = user.driverLicenseReceiveDate;
            RentalName = Constants.RentalName;
        }
    }
    public class OfferRequestFront
    {
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public string PlannedStartDate { get; set; }
        public string PlannedEndDate { get; set; }
    }
}
