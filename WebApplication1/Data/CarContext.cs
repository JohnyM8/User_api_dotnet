using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class CarContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public CarContext(DbContextOptions<CarContext> options)
            : base(options)
        {

        }
    }
}