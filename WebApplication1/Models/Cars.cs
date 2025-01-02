using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Car
    {
        [Key]
        public int id { get; set; }
        public string? rentalService = "My";
        public string? producer { get; set; }
        public string? model { get; set; }
        public string? type { get; set; }
        public string? location { get; set; }
        public string yearOfProduction { get; set; }
        public int numberOfSeats { get; set; }
        public int IsAvailable { get; set; }

        public Car() 
        {
        
        }

        public Car(CarDtoNew data)
        {
            id = data.Id;
            //rentalService = data.rentalService;
            producer = data.Producer;
            model = data.Model;
            location = data.Location;
            yearOfProduction = $"{data.YearOfProduction}";
            numberOfSeats = data.NumberOfSeats;
            type = data.Type.ToString();
            if (data.IsAvailable)
                IsAvailable = 1;
            else
                IsAvailable = 0;
        }

        public Car(CarDto data)
        {
            id = data.id;
            //rentalService = data.rentalService;
            producer = data.producer;
            model = data.model;
            location = data.location;
            yearOfProduction = $"{data.yearOfProduction}";
            numberOfSeats = data.numberOfSeats;
            type = data.type.ToString();
            if (data.isAvailable)
                IsAvailable = 1;
            else
                IsAvailable = 0;
        }

    }
    public class CarDtoNew
    {
        public int Id { get; set; }
        public string Producer { get; set; }
        public string Model { get; set; }
        public int YearOfProduction { get; set; }
        public int NumberOfSeats { get; set; }
        public CarType Type { get; set; }
        public bool IsAvailable { get; set; }
        // public GearboxType Gearbox { get; set; }
        // public FuelType Fuel { get; set; }
        public string Location { get; set; }

        public CarDtoNew() { }

        public CarDtoNew(Car data)
        {
            Id = data.id;
            //rentalService = data.rentalService;
            Producer = data.producer;
            Model = data.model;
            Location = data.location;
            YearOfProduction = int.Parse(data.yearOfProduction);
            NumberOfSeats = data.numberOfSeats;
            if (data.IsAvailable == 0)
                IsAvailable = false;
            else
                IsAvailable = true;
        }
    }

    public class CarDto
    {
        public int id { get; set; }
        public string? producer { get; set; }
        public string? model { get; set; }
        public int yearOfProduction { get; set; }
        public int numberOfSeats { get; set; }
        public int type { get; set; }
        public bool isAvailable { get; set; }
        public string? location { get; set; }
        
        public CarDto()
        { }
        

        public CarDto(Car data)
        {
            id = data.id;
            //rentalService = data.rentalService;
            producer = data.producer;
            model = data.model;
            location = data.location;
            yearOfProduction = int.Parse(data.yearOfProduction);
            numberOfSeats = data.numberOfSeats;
            if (data.IsAvailable == 0)
                isAvailable = false;
            else
                isAvailable = true;
        }
    }
}
