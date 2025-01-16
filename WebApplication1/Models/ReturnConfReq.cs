using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ReturnRecord
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public int EmployeeID { get; set; }
        public string Condition { get; set; }
        public string FrontPhotoUrl { get; set; }
        public string BackPhotoUrl { get; set; }

        public string RightPhotoUrl { get; set; }

        public string LeftPhotoUrl { get; set; }
        public string EmployeeNotes { get; set; }
        public DateTime ReturnDate { get; set; }
    }
    public class ReturnConfReq
    {
        public int? UserId { get; set; }
        public string? EmployeeNotes { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? OfferId { get; set; }
    }
}
