namespace WebApplication1.Models
{
    public static class StringExtensions
    {
        public static CarType ToCarType(this string data)
        {
            CarType carTypeMultiplier = data switch
            {
                "compact" => CarType.compact,
                "economy" => CarType.economy,
                "van" => CarType.van,
                "Van" => CarType.van,
                "suv" => CarType.suv,
                "Suv" => CarType.suv,
                // CarType.Luxury => 3.0,
                // CarType.Sports => 3.5,
                _ => CarType.other // Default for unknown car types
            };

            return carTypeMultiplier;
        }
    }
    public enum CarType
    {
        compact,
        economy,
        van,
        suv,
        other
    }
    public enum RentalStatus
    {
        planned,
        inProgress,
        pendingReturn,
        ended
    }
}
