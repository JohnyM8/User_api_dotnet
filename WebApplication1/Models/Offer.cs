using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Offer
    {
        [JsonIgnore]
        public static int _id = 0;
        public int id { get; set; }
        public int carID { get; set; }
        public decimal dayRate { get; set; }
        public decimal insuranceRate { get; set; }
        public DateTime validUntil { get; set; }
    }
}
