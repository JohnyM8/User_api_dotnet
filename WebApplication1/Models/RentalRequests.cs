using System.Reflection.Metadata;

namespace WebApplication1.Models
{
    public class RentalRequestDto
    {
        public int OfferId { get; set; } // Moje id
        public int CustomerId { get; set; } // External id

        public string RentalName { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }

        public RentalRequestDto(RentalRequestFront data)
        {
            OfferId = int.Parse(data.OfferId);
            CustomerId = int.Parse(data.CustomerId);
            //RentalName = "My";
            RentalName = Constants.RentalName;
            PlannedStartDate = DateTime.Parse(data.PlannedStartDate);
            PlannedEndDate = DateTime.Parse(data.PlannedEndDate);
        }
    }
    public class RentalRequestFront
    {
        public string OfferId { get; set; } // Moje id
        public string CustomerId { get; set; } // External id
        public string RentalName { get; set; }
        public string PlannedStartDate { get; set; }
        public string PlannedEndDate { get; set; }
    }
}
