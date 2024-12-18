using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public ApiContext(DbContextOptions<ApiContext> options)
            :base(options)
        {

        }

        public User? FindByLogin(string login)
        {
            var tmp = Users.Where(user => user.login == login).ToArray();

            if (tmp.Count() == 0)
                return null;
            else
                return tmp[0];

        }
        public User? FindByEmail(string email)
        {
            var tmp = Users.Where(user => user.email == email).ToArray();

            if(tmp.Count() == 0)
                return null;
            else 
                return tmp[0];
             
        }
    }
}
