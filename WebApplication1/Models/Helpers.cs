using System.Threading.Tasks;

namespace WebApplication1.Models
{


    public static class Constants
    {
        public const string RentalName = "JEJ Car Rental";

        public const string RentalName2 = "Some name";
    }
    public static class CarListExtensions
    {
        public static IEnumerable<Car> GetPage(this IEnumerable<Car> array , int page)
        {
            if (page < 1)
                return null;

            int pages = array.CountPages();

            var tmp = array.ToArray();

            int count = tmp.Length;

            if (count < 6)
                return tmp;
            if(pages <= page)
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

        public static Car? FindCarById(this IEnumerable<Car> list , string Id)
        {
            var newList = new List<Car>();

            var tmp = newList.Where(car => car.id == Id).ToArray();

            if (tmp == null || tmp.Count() == 0)
                return null;
            else
                return tmp[0];
        }
    }
}
