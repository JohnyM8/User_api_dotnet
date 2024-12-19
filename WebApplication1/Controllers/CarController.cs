using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Web;
using System.Text;
using System.Text.Json;
using WebApplication1.Data;
using System.Data.Odbc;
using System.Net.Http;
using NuGet.Common;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Azure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Composition;

namespace WebApplication1.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ApiContext _context;

        static readonly string forwordURL = @"https://car-rental-api-chezbchwebfggwcd.canadacentral-01.azurewebsites.net";
        private readonly HttpClient _httpClient;
        private string? token = null;
        static readonly string login = "JejRental";
        static readonly string password = "q3409tg-hoij";
        //public CarController(ApiContext context)
        //{
        //    _context = context;
        //}

        public CarController(HttpClient httpClient , ApiContext context)
        {
            _httpClient = httpClient;
            _context = context;
            //Task<string> task = Task<string>.Run(async () => await GetToken());
            //token = task.Result;
        }
        
        public async Task<string> GetToken()
        {
            var creds = new Dictionary<string, string>
            {
                { "Username", login },
                { "Password", password },
            };

            string json = JsonConvert.SerializeObject(creds);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(forwordURL + "/api/auth/login", httpContent);

            //var responseContent = await httpResponse.Content.ReadFromJsonAsync<LoginResponseFromDto>();

            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (responseContent == null)
                return ""; //StatusCode(500, "bad read");

            return responseContent;
        }

        [HttpGet("getToken")]
        public async Task<ActionResult<string>> GetTokenEndPoint()
        {
            var creds = new LoginToDto() { Username = login, Password = password };
            /*
            var creds = new Dictionary<string, string>
            {
                { "Username", login },
                { "Password", password },
            };
            */
            string json = JsonConvert.SerializeObject(creds);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(forwordURL + "/api/auth/login", httpContent);

            //var responseContent = await httpResponse.Content.ReadFromJsonAsync<LoginResponseFromDto>();

            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (responseContent == null)
                return NotFound(); //StatusCode(500, "bad read");

            return StatusCode(200 , responseContent);
        }
        [HttpGet("getAllAvailable")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllAvailableCars()
        {
            return StatusCode(200 , _context.AllAvailable());

            //var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/user/cars");

            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            //var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            //return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpGet("getAllCars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllCars()
        {
            return StatusCode(200, _context.All());
            //var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/user/cars");

            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            //var responseContent = await response.Content.ReadAsStringAsync();

            //return StatusCode((int)response.StatusCode, responseContent);
        }


        [HttpPost("getPage")]
        public async Task<ActionResult<IEnumerable<Car>>> GetPage([FromBody] PageRequest data)
        {
            return StatusCode(200, _context.GetPage(data.Page));
        }

        [HttpGet("getCountPages")]
        public async Task<ActionResult<int>> GetCountPage()
        {
            return StatusCode(200, _context.GetNumPages());
        }

        [HttpPost("getBathCars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetBathCars([FromBody] BathCarRequest data)
        {


            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/user/cars");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var responseContent = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, responseContent);
        }

        [Authorize]
        [HttpPost("getOffer")]
        public async Task<ActionResult<RentalOfferDto>> GetOffer([FromBody] OfferRequestFront data)
        {
            var RentalObj = new OfferRequestDto(data);

            var User = _context.FindUserById(data.CustomerId);

            if (User == null)
                return NotFound("User not found");

            var Car = _context.FindCarById(data.CarId);

            if(Car == null)
                return NotFound("Car not found");

            var offer = new RentalOffer
            {
                userId = User.id,
                carId = data.CarId,
                dailyRate = CarRentalCalculator.CalculateDailyCarRate(new CarDto(Car), new UserDtoToDataB(User)),
                insuranceRate = CarRentalCalculator.CalculateDailyInsuranceRate(new CarDto(Car), new UserDtoToDataB(User)),
                validUntil = DateTime.UtcNow.AddMinutes(10),
                isActive = true
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            if (EmailSender.SendOfferEmail(User.email))
                return BadRequest("Email didnt sent");

            return Ok(offer);
            ////var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals/offers");

            ////request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            ////request.Content = JsonContent.Create(RentalObj);

            ////HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            //////var tmp_content = JsonConvert.SerializeObject(RentalObj);

            //////var content = new StringContent(
            //////JsonConvert.SerializeObject(RentalObj),
            //////Encoding.UTF8,
            //////"application/json");

            //////var response = await _httpClient.PostAsync(forwordURL + "/api/customer/rentals/offers" , content);

            ////var responseContent = await response.Content.ReadFromJsonAsync<RentalOfferDto>();

            ////return StatusCode((int)response.StatusCode, responseContent);
        }
        [Authorize]
        [HttpPost("rent")]
        public async Task<ActionResult<RentalToFront>> GetRent([FromBody] RentalRequestFront data)
        {

            var offer = _context.Offers.Find(data.OfferId);

            if(offer == null)
                return NotFound("Offer not found");

            var new_data = new RentalRequestDto(data);

            var rental = new Rental
            {
                carId = offer.carId,
                userId = data.CustomerId,
                startDate = new_data.PlannedStartDate,
                endDate = new_data.PlannedEndDate,
                totalPrice = (offer.dailyRate + offer.insuranceRate) *
                         (new_data.PlannedEndDate - new_data.PlannedStartDate).Days,
                status = new_data.PlannedStartDate > DateTime.UtcNow ? RentalStatus.planned : RentalStatus.inProgress
            };

            var car = _context.Cars.Find(rental.carId);

            if (car == null)
                return NotFound("Car not found");

            car.IsAvailable = 0;
            _context.Rentals.Add(rental);
            offer.isActive = false;
            await _context.SaveChangesAsync();



            EmailSender.SendRentEmail(_context.GetUserEmailById(data.CustomerId));

            return Ok(new RentalToFront(rental));

            //var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals");

            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //var RentalObj = new RentalRequestDto(data);

            //request.Content = JsonContent.Create(RentalObj);

            //HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);



            ////var RentalObj = new RentalRequestDto(data);


            ////var content = new StringContent(
            ////Newtonsoft.Json.JsonConvert.SerializeObject(RentalObj),
            ////Encoding.UTF8,
            ////"application/json");


            ////var response = await _httpClient.PostAsync(forwordURL + "/api/customer/rentals", content);

            //var responseContent = await response.Content.ReadFromJsonAsync<RentalRequestDto>();

            //return StatusCode((int)response.StatusCode, responseContent);
        }
        [Authorize]
        [HttpPost("return")]
        public async Task<ActionResult<RentalToFront>> ReturnCar([FromBody] ReturnRequestFront data)
        {
            var rental = _context.Rentals.Find(data.RentalId);

            if (rental == null)
                return NotFound("Rental offer not found");

            else if (rental.status == RentalStatus.pendingReturn)
                return NotFound("Rental arleady peding return");

            else if (rental.status == RentalStatus.planned)
                return NotFound("Rental yet to be planned");

            else if (rental.status == RentalStatus.ended)
                return NotFound("Rental arleady ended");

            rental.status = RentalStatus.pendingReturn;

            await _context.SaveChangesAsync();

            EmailSender.SendReturnStartEmail(_context.GetUserEmailById(rental.userId));

            return Ok(new RentalToFront(rental));

            //var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals");

            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //var RentalObj = new ReturnRequestDto(data);

            //request.Content = JsonContent.Create(RentalObj);

            //HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);



            ////var RentalObj = new RentalRequestDto(data);


            ////var content = new StringContent(
            ////Newtonsoft.Json.JsonConvert.SerializeObject(RentalObj),
            ////Encoding.UTF8,
            ////"application/json");


            ////var response = await _httpClient.PostAsync(forwordURL + "/api/customer/rentals", content);

            //var responseContent = await response.Content.ReadFromJsonAsync<RentalRequestDto>();

            //return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpPost("rentedCars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetRentedCars([FromBody] RentedCarsRequest data)
        {
            var rentals = _context.GetUserRentals(data.UserId);

            if (rentals.Count() == 0)
                return NotFound("User has not rented any cars");

            List<Car> CarList = new List<Car>();

            foreach (var rental in rentals)
            {
                var car = _context.Cars.Find(rental.carId);

                if(car != null && !CarList.Contains(car))
                    CarList.Add(car);
            }

            return Ok(CarList);

            //var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals");

            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //var RentalObj = new ReturnRequestDto(data);

            //request.Content = JsonContent.Create(RentalObj);

            //HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);



            ////var RentalObj = new RentalRequestDto(data);


            ////var content = new StringContent(
            ////Newtonsoft.Json.JsonConvert.SerializeObject(RentalObj),
            ////Encoding.UTF8,
            ////"application/json");


            ////var response = await _httpClient.PostAsync(forwordURL + "/api/customer/rentals", content);

            //var responseContent = await response.Content.ReadFromJsonAsync<RentalRequestDto>();

            //return StatusCode((int)response.StatusCode, responseContent);
        }
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCars(int id)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + $"/api/user/cars/{id}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            //if(response == null)
            var responseContent = await response.Content.ReadFromJsonAsync<CarDto>();

            return StatusCode((int)response.StatusCode, responseContent);
        }
        /*
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCars(int id, User user)
        {
            return new RedirectResult(forwordURL);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostCar(User user)
        {
            return new RedirectResult(forwordURL);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            return new RedirectResult(forwordURL);
        }
        */
    }
}
