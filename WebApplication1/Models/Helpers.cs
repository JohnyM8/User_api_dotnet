namespace WebApplication1.Models
{
    public class CarRentalCalculator
    {
        public const double DefaultBaseRate = 80.0; // Base daily car rental rate in dollars
        public const double DefaultInsuranceRate = 15.0; // Base daily insurance rate in dollars


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
            double driverAgeMultiplier = DateTime.Now.Year - customer.birthday.Value.Year switch
            {
                <= 21 => 1.5, // Young drivers pay more
                <= 25 => 1.2,
                >= 65 => 1.3, // Senior drivers pay slightly more
                _ => 1.0
            };

            return (decimal)(DefaultBaseRate * carTypeMultiplier * ageMultiplier * driverAgeMultiplier);
        }

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
            double driverAgeMultiplier = DateTime.Now.Year - customer.birthday.Value.Year switch
            {
                <= 21 => 1.5, // Young drivers pay more
                <= 25 => 1.2,
                >= 65 => 1.3, // Senior drivers pay slightly more
                _ => 1.0
            };

            return (decimal)(DefaultInsuranceRate * carTypeMultiplier * ageMultiplier * driverAgeMultiplier);
        }
    }
}
