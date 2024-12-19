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
        public string yearOfProduction { get; set; }
        public int? numberOfSeats { get; set; }
        public int? IsAvailable { get; set; }

    }
    public class CarDto
    {
        public int id { get; set; }
        public string? rentalService { get; set; }
        public string? producer { get; set; }
        public string? model { get; set; }
        public CarType? type { get; set; }
        public string? location { get; set; }
        public int yearOfProduction { get; set; }
        public int? numberOfSeats { get; set; }
        public int? IsAvailable { get; set; }

        public CarDto(Car data)
        {
            id = data.id;
            rentalService = data.rentalService;
            producer = data.producer;
            model = data.model;
            location = data.location;
            yearOfProduction = int.Parse(data.yearOfProduction);
            numberOfSeats = data.numberOfSeats;
            IsAvailable = data.IsAvailable;
        }
    }
}
