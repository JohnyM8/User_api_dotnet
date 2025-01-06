using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<RentalOffer> Offers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }
        public string GetUserEmailById(int id)
        {
            var User = Users.Find(id);

            return User.email;
        }

        public User? FindUserById(int id)
        {
            var tmp = Users.Where(user => user.id == id).ToArray();

            if (tmp.Count() == 0)
                return null;

            return tmp[0];
        }


        public User? FindByLogin(string login)
        {
            var tmp = Users.Where(user => user.login == login).ToArray();

            if (tmp == null)
                return null;
            else
                return tmp[0];

        }
        public User? FindByEmail(string email)
        {
            var tmp = Users.Where(user => user.email == email).ToArray();

            if(tmp == null)
                return null;
            else 
                return tmp[0];
             
        }
        //public IEnumerable<Rental>? GetUserRentals(int UserId)
        //{
        //    var tmp = Rentals.Where(rental => rental.userId == UserId);

        //    if (tmp == null)
        //        return null;

        //    return tmp.ToArray();
        //}

        //public Car? FindCarById(int id)
        //{
        //    var tmp = Cars.Where(car => car.id == id).ToArray();

        //    if (tmp.Count() == 0)
        //        return null;

        //    return tmp[0];
        //}
        //public int GetNumPages()
        //{
        //    var tmp = AllAvailable().ToArray();

        //    int max = tmp.Length;

        //    if (max < 1)
        //        return 0;

        //    return (max - 1) / 5 + 1;
        //}

        //public IEnumerable<Car> GetPage(int i)
        //{
        //    var tmp = AllAvailable().ToArray();

        //    int max = tmp.Length;

        //    if(max < 6)
        //        return tmp;

        //    if(max < i * 5 && max > (i - 1) * 5)
        //    {
        //        return tmp[(5 * (i - 1))..(max)];
        //    }
        //    else if(max <= (i - 1) * 5)
        //    {
        //        int maxPage = (max - 1) / 5;
        //        return tmp[(5 * maxPage)..(max)];
        //    }

        //    return tmp[(5 * (i - 1))..(5 * i)];
        //}
        //public IEnumerable<Car> All()
        //{
        //    var tmp = Cars.ToArray();

        //    return tmp;
        //}

        //public IEnumerable<Car> AllAvailable()
        //{
        //    var tmp = Cars.Where(car => car.IsAvailable == 1).ToArray();

        //    return tmp;
        //}
    }
}
