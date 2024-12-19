namespace WebApplication1.Models
{
    public class PageRequest
    {
        public int Page { get; set; }
    }
    public class BathCarRequest
    {
        public int Page { get; set; }
        public int MaxPages { get; set; }
        public IEnumerable<string>? rentalServices { get; set; }
        public IEnumerable<string>? producers { get; set; }
        public IEnumerable<string>? modeles { get; set; }
        public IEnumerable<string>? types { get; set; }
        public IEnumerable<string>? numbersOfSeats { get; set; }
        public IEnumerable<string>? locations { get; set; }
        public IEnumerable<string>? yearsOfProduction { get; set; }
        public IEnumerable<string>? availability { get; set; }

    }
}
