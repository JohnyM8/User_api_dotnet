namespace WebApplication1.Models
{
    public class ReturnRequestDto
    {
        public int RentalId { get; set; }

        public ReturnRequestDto(ReturnRequestFront data)
        {
            RentalId = data.RentalId;
        }
    }
    public class ReturnRequestFront
    {
        // public int Id { get; set; }
        public int UserId { get; set; }
        public int RentalId { get; set; }
    }

    public class ReturnRecordDto
    {
        // public int Id { get; set; }
        public int RentalId { get; set; }
        public string Condition { get; set; }
        // public string PhotoUrl { get; set; }
        public string EmployeeNotes { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
