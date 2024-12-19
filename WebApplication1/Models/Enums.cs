namespace WebApplication1.Models
{
    public static class StringExtensions
    {
        //public static string ToString(this CarType data)
        //{
        //    return data.ToString();
        //}
    }
    public enum CarType
    {
        compact,
        economy,
        van,
        suv
    }
    public enum RentalStatus
    {
        planned,
        inProgress,
        pendingReturn,
        ended
    }
}
