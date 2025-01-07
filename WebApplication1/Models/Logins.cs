namespace WebApplication1.Models
{
    public class LoginCredential
    {
        public string? login { get; set; }
        public string? password { get; set; }
    }

    public class LoginResponseFromDto
    {
        public string? token { get; set; }
    }

    public class LoginToDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
