using System.ComponentModel.DataAnnotations;


namespace WebApplication1.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string? firstname { get; set; }
        public string? email { get; set; }
        public string? lastname { get; set; }
        public string? login { get; set; }
        public string? password { get; set; }
        public string? rentalService { get; set; }
        public string? birthday { get; set; }
        public string? driverLicenseReceiveDate { get; set; }
    }

    public class UserDto
    {
        [Key]
        public int id { get; set; }
        public string? firstname { get; set; }
        public string? email { get; set; }
        public string? lastname { get; set; }
        public string? login { get; set; }
        public string? password { get; set; }
        public string? rentalService { get; set; }
        public DateTime? birthday { get; set; }
        public string? driverLicenseReceiveDate { get; set; }

        public UserDto(User data)
        {
            id = data.id;
            firstname = data.firstname;
            email = data.email;
            lastname = data.lastname;
            login = data.login;
            password = data.password;
            rentalService = data.rentalService;
            birthday = DateTime.Parse(data.birthday);
            driverLicenseReceiveDate = data.driverLicenseReceiveDate;
        }
    }

    public class UserToFront
    {
        [Key]
        public int id { get; set; }
        public string? firstname { get; set; }
        public string? email { get; set; }
        public string? lastname { get; set; }
        public string? login { get; set; }
        public string? password { get; set; }
        public string? rentalService { get; set; }
        public string? birthday { get; set; }
        public string? driverLicenseReceiveDate { get; set; }

        public string? CertificateToken { get; set; }

        public UserToFront(User user)
        {
            this.id = user.id;
            this.firstname = user.firstname;
            this.email = user.email;
            this.lastname = user.lastname;
            this.login = user.login;
            this.password = user.password;
            this.rentalService = user.rentalService;
            //this.birthday = user.birthday;
            this.driverLicenseReceiveDate = user.driverLicenseReceiveDate;
        }
    }
}
