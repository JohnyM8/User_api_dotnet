using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Text.Json;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiContext _context;
        private GoogleAuthService _googleAuthService;
        private IConfiguration _configuration;

        public UsersController(ApiContext context , HttpClient httpClient , IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _googleAuthService = new GoogleAuthService(httpClient , configuration);
        }
        

        [HttpPost("google")]
        public async Task<ActionResult<AuthResponse>> GoogleAuth([FromBody] GoogleAuthRequest request)
        {
            // Weryfikacja kodu Google
            var googleTokens = await _googleAuthService.ExchangeCodeForTokens(request.Code!, request.RedirectUri!);

            // Pobranie informacji o użytkowniku
            //var googleUserInfo = await _googleAuthService.GetUserInfo(googleTokens.access_token!);

            var googleUserInfo = _googleAuthService.GetUserInfoFromIdToken(googleTokens.id_token);

            // Utworzenie/znalezienie użytkownika
            var user = _context.FindByEmail(googleUserInfo.Email!);

            // Wygenerowanie tokenu JWT
            var token = TokenManager.GenerateJwtToken(_configuration);

            return Ok(new AuthResponse
            {
                Token = token,
                User = new UserDtoGoogle(googleUserInfo),
                IsNewUser = user == null,
            });
        }

        [Authorize]
        [HttpPost("google/moreData")]
        public async Task<ActionResult<AuthRegisterResponse>> GoogleAuthMoreData([FromBody] GoogleAuthRegisterRequest request)
        {
            if (_context.FindByLogin(request.login) != null)
                return BadRequest("User with that login arleady exists");

            if (_context.FindByEmail(request.Email) != null)
                return BadRequest("User with that email arleady exists");

            User newUser = new User()
            {
                login = request.login,
                password = "",
                email = request.Email,
                firstname = request.firstname,
                lastname = request.lastname,
                rentalService = Constants.RentalName,
                birthday = request.birthday,
                driverLicenseReceiveDate = request.driverLicenseReceiveDate,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new AuthRegisterResponse()
            {
                User = newUser,
                IsProfileComplete = true
            });
        }

        [Authorize]
        [HttpPost("google/reset")]
        public async Task<ActionResult<TokenResetResponse>> TokenReset()
        {
            // Wygenerowanie tokenu JWT
            var token = TokenManager.GenerateJwtToken(_configuration);

            return Ok(new TokenResetResponse
            {
                Token = token,
            });
        }

        //GARBAGE ONLY BELOW//





        [HttpPost("google2")]
        public async Task<ActionResult<string>> GoogleGetJson1([FromBody] GoogleAuthRequest request)
        {
            // Weryfikacja kodu Google
            var googleTokens = _googleAuthService.GetJsonString(request.Code, request.RedirectUri);

            return Ok(googleTokens);
        }

        [HttpGet("google3")]
        public async Task<ActionResult<string>> GoogleGetJson2()
        {
            // Weryfikacja kodu Google
            var googleTokens = TokenManager.GenerateJwtToken(_configuration);

            return Ok(googleTokens);
        }

        //[HttpPost("google/signUp")]
        //public async Task<ActionResult<AuthRegisterResponse>> GoogleAuthRegister([FromBody] GoogleAuthRegisterRequest request)
        //{
        //    // Weryfikacja kodu Google
        //    var googleTokens = await _googleAuthService.ExchangeCodeForTokens(request.Code!, request.RedirectUri!);

        //    // Pobranie informacji o użytkowniku
        //    //var googleUserInfo = await _googleAuthService.GetUserInfo(googleTokens.access_token!);

        //    var googleUserInfo = _googleAuthService.GetUserInfoFromIdToken(googleTokens.id_token);

        //    // Utworzenie/znalezienie użytkownika
        //    //var user = _context.FindByEmail(googleUserInfo.Email!);

        //    // Wygenerowanie tokenu JWT
        //    var token = TokenManager.GenerateJwtToken(_configuration);

        //    User newUser = new User()
        //    {
        //        login = request.login,
        //        password = "",
        //        email = googleUserInfo.Email,
        //        firstname = request.firstname,
        //        lastname = request.lastname,
        //        rentalService = "0",
        //        birthday = request.birthday,
        //        driverLicenseReceiveDate = request.driverLicenseReceiveDate,
        //    };

        //    _context.Users.Add(newUser);
        //    await _context.SaveChangesAsync();

        //    return Ok(new AuthRegisterResponse()
        //    {
        //        //Token = token,
        //        User = newUser,
        //    });

        //}

        [HttpPost("signIn")]
        public async Task<ActionResult<UserToFront?>> GetCredentials([FromBody] LoginCredential data)
        {

            // Dostęp do wartości
            if (data == null)
                return NotFound("No data found");

            string login = data.login!;
            string password = data.password!;


            var tmp = _context.Users.Where(user => user.login == login).ToArray();

            if (tmp.Count() < 1)
                return NotFound("User with that login does not exist");

            var user = tmp[0];
            if (user.password != password)
                return NotFound("Wrong password");

            var usertof = new UserToFront(user);

            //string token = TokenManager.GenerateJwtToken();
            //usertof.CertificateToken = token;

            return usertof;
        }



        [HttpPost("signOn")]
        public async Task<ActionResult<UserToFront?>> Register([FromBody] RegisterCredential data)
        {

            // Dostęp do wartości
            if (data == null)
                return NotFound("No data found");

            string login = data.login!;
            string password = data.password!;
            string email = data.email!;


            var tmp = _context.FindByLogin(login);

            if (tmp != null)
                return NotFound("User with that login arleady exist");

            tmp = _context.FindByEmail(email);

            if (tmp != null)
                return NotFound("User with that email arleady exist");

            User newUser = new User()
            {
                login = login,
                password = password,
                email = email,
                firstname = data.firstname,
                lastname = data.lastname,
                rentalService = "0",
                birthday = data.birthday,
                driverLicenseReceiveDate = data.driverLicenseReceiveDate,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            tmp = _context.FindByLogin(login);

            if (tmp == null)
                return NotFound();

            var usertof = new UserToFront(tmp);

            //string token = TokenManager.GenerateJwtToken();
            //usertof.CertificateToken = token;

            return usertof;
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUsers(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestOld emailRequest)
        {
            //if (string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Subject) || string.IsNullOrEmpty(emailRequest.Body))
            //{
            //    return BadRequest("Email request must include recipient, subject, and body.");
            //}

            //try
            //{
            //    MimeMessage message = new MimeMessage();

            //    message.From.Add(new MailboxAddress("Admin", "user.api.adnin@gmail.com"));

            //    message.To.Add(MailboxAddress.Parse( emailRequest.To));

            //    message.Subject = emailRequest.Subject;

            //    message.Body = new TextPart("plain")
            //    {
            //        Text = emailRequest.Body
            //    };

            //    using (var smtpClient = new SmtpClient()) // Podaj swój SMTP serwer
            //    {
            //        smtpClient.CheckCertificateRevocation = false;
            //        smtpClient.Connect("smtp.gmail.com", 465, true);

            //        smtpClient.Authenticate("user.api.adnin@gmail.com", "wbrz wood sdcd ygib");

            //        smtpClient.Send(message);
            //    }

            //    return Ok("Email sent successfully!");
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, $"Internal server error: {ex.Message}");
            //}

            RentalOfferDto rental = new RentalOfferDto()
            {
                Id = 1,
                DailyRate = 10,
                InsuranceRate = 10,
                TotalCost = 10,
                ValidUntil = DateTime.UtcNow,
            };

            var offer = new OfferRequestDto() 
            { 
                CarId = 1,
                CustomerId = 1,
                PlannedEndDate = DateTime.Parse("2025.12.20"),
                PlannedStartDate = DateTime.Parse("2025.10.12")

            };

            if (!EmailSender.SendOfferEmail(emailRequest.To , rental , offer))
                return BadRequest("Email wasnt send");

            return Ok();
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }
        
    }
}
