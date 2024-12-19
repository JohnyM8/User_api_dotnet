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

        public OfferRequestDto(OfferRequestFront data)
        {
            CarId = data.CarId;
            CustomerId = data.CustomerId;
            //firstName = data.firstName;
            //lastName = data.lastName;
            //birthday = DateOnly.Parse(data.birthday);
            //driverLicenseReceiveDate = DateOnly.Parse(data.driverLicenseReceiveDate);
            //RentalName = data.RentalName;
            PlannedStartDate = DateTime.Parse(data.PlannedStartDate);
            PlannedEndDate = DateTime.Parse(data.PlannedEndDate);
        }
    }
    public class OfferRequestFront
    {
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        //public string firstName { get; set; }
        //public string lastName { get; set; }
        //public string birthday { get; set; }
        //public string driverLicenseReceiveDate { get; set; }
        //public string RentalName { get; set; }
        public string PlannedStartDate { get; set; }
        public string PlannedEndDate { get; set; }
        
        /*
        public string? startdate { get; set; }
        public string? enddate { get; set; }
        public Car? car { get; set; }
        */
    }
}
