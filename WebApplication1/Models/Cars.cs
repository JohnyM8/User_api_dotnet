using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Car
    {
        [Key]
        public int id { get; set; }
        public string? rentalService { get; set; }
        public string? producer { get; set; }
        public string? model { get; set; }
        public string? type { get; set; }
        public string? location { get; set; }
        public string? yearOfProduction { get; set; }
        public int? numberOfSeats { get; set; }
        public int? IsAvailable { get; set; }

    }
    public class CarDto
    {
        public int Id { get; set; }
        public string? RentalService = "My";
        public string? Producer { get; set; }
        public string? Model { get; set; }
        public int YearOfProduction { get; set; }
        public int NumberOfSeats { get; set; }
        //public CarType Type { get; set; }
        public bool IsAvailable { get; set; }
        // public GearboxType Gearbox { get; set; }
        // public FuelType Fuel { get; set; }
        public string? Location { get; set; }
    }
}
