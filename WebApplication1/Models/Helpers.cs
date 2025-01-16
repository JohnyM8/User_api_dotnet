using System.Threading.Tasks;

namespace WebApplication1.Models
{


    public static class Constants
    {
        public const string RentalName = "JEJ Car Rental";

        public const string RentalName2 = "GMM Car Rental";

        public const string RentalNameTo2 = "EKJCD";
    }
    public static class CarListExtensions
    {
        public static IEnumerable<Car> GetPage(this IEnumerable<Car> array, int page)
        {
            if (page < 1)
                return null;

            int pages = array.CountPages();

            var tmp = array.ToArray();

            int count = tmp.Length;

            if (count < 6)
                return tmp;
            if (pages <= page)
                return tmp[(5 * pages)..(count)];

            return tmp[(5 * (page - 1))..(5 * page)];
        }
        public static int CountPages(this IEnumerable<Car> array)
        {
            var CarList = array.ToArray();

            if (CarList == null)
                return -1;

            int max = CarList.Length;

            if (max < 1)
                return 0;

            return (max - 1) / 5 + 1;
        }
        public static IEnumerable<Car> ConvertToCar(this IEnumerable<CarDtoNew> list)
        {
            var newList = new List<Car>();

            foreach (var item in list)
            {
                newList.Add(new Car(item));
            }

            return newList;
        }
        public static IEnumerable<Car> ConvertToCar(this IEnumerable<CarDto> list)
        {
            var newList = new List<Car>();

            foreach (var item in list)
            {
                newList.Add(new Car(item));
            }

            return newList;
        }

        public static IEnumerable<Car> ConvertToCar(this IEnumerable<CarDto2> list)
        {
            var newList = new List<Car>();

            foreach (var item in list)
            {
                newList.Add(new Car(item));
            }

            return newList;
        }

        public static Car? FindCarById(this IEnumerable<Car> list, string Id)
        {
            var newList = new List<Car>();

            var tmp = newList.Where(car => car.id == Id).ToArray();

            if (tmp == null || tmp.Count() == 0)
                return null;
            else
                return tmp[0];
        }
    }
    public static class Count
    {

        public static decimal CalculateDailyInsuranceRate(CarDto car, UserDto customer)
        {

            // Later we could add calculating based on previous rentals (for example add score field for every customer)
            double carTypeMultiplier = car.type switch
            {
                CarType.compact => 1.0,
                CarType.economy => 1.5,
                CarType.van => 1.8,
                CarType.suv => 2.0,
                // CarType.Luxury => 3.0,
                // CarType.Sports => 3.5,
                _ => 2.0 // Default for unknown car types
            };
            double carAge = DateTime.Now.Year - car.yearOfProduction;
            double ageMultiplier = carAge < 3 ? 1.0 : (carAge < 7 ? 0.9 : 0.8);
            // double gearMultiplier = car.Gearbox == GearboxType.Automatic ? 1.2 : 1.0;
            // double fuelMultiplier = car.FuelType switch
            // {
            //     FuelType.Petrol => 1.1,
            //     FuelType.Diesel => 1.0,
            //     FuelType.Hybrid => 1.2,
            //     FuelType.Electric => 1.5,
            //     FuelType.Hydrogen => 2.0,
            //     _ => 1.0
            // };
            double driverAgeMultiplier = DateTime.Now.Year - customer.birthday.Year switch
            {
                <= 21 => 1.5, // Young drivers pay more
                <= 25 => 1.2,
                >= 65 => 1.3, // Senior drivers pay slightly more
                _ => 1.0
            };

            return (decimal)(15 * carTypeMultiplier * ageMultiplier * driverAgeMultiplier);
        }
        public static decimal CalculateDailyCarRate(CarDto car, UserDto customer)
        {
            // Later we could add calculating based on previous rentals (for example add score field for every customer)
            double carTypeMultiplier = car.type switch
            {
                CarType.compact => 1.0,
                CarType.economy => 1.5,
                CarType.van => 1.8,
                CarType.suv => 2.0,
                // CarType.Luxury => 3.0,
                // CarType.Sports => 3.5,
                _ => 2.0 // Default for unknown car types
            };
            double carAge = DateTime.Now.Year - car.yearOfProduction;
            double ageMultiplier = carAge < 3 ? 1.0 : (carAge < 7 ? 0.9 : 0.8);
            // double gearMultiplier = car.Gearbox == GearboxType.Automatic ? 1.2 : 1.0;
            // double fuelMultiplier = car.FuelType switch
            // {
            //     FuelType.Petrol => 1.1,
            //     FuelType.Diesel => 1.0,
            //     FuelType.Hybrid => 1.2,
            //     FuelType.Electric => 1.5,
            //     FuelType.Hydrogen => 2.0,
            //     _ => 1.0
            // };
            double driverAgeMultiplier = DateTime.Now.Year - customer.birthday.Year switch
            {
                <= 21 => 1.5, // Young drivers pay more
                <= 25 => 1.2,
                >= 65 => 1.3, // Senior drivers pay slightly more
                _ => 1.0
            };

            return (decimal)(15 * carTypeMultiplier * ageMultiplier * driverAgeMultiplier);
        }
    }
}