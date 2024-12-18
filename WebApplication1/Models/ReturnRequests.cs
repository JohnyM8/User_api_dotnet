namespace WebApplication1.Models
{
    public class ReturnRequestDto
    {
        // public int Id { get; set; }
        public int RentalId { get; set; }
        public string Condition { get; set; }
        // public string PhotoUrl { get; set; }
        public string EmployeeNotes { get; set; }
        public DateTime ReturnDate { get; set; }

        public ReturnRequestDto(ReturnRequestFront data)
        {
            RentalId = data.RentalId;
            Condition = data.Condition;
            EmployeeNotes = data.EmployeeNotes;
            ReturnDate = DateTime.Parse(data.ReturnDate);
        }
    }
    public class ReturnRequestFront
    {
        // public int Id { get; set; }
        public int RentalId { get; set; }
        public string Condition { get; set; }
        // public string PhotoUrl { get; set; }
        public string EmployeeNotes { get; set; }
        public string ReturnDate { get; set; }
    }
}
