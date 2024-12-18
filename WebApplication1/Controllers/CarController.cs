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

namespace WebApplication1.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ApiContext _context;

        static readonly string forwordURL = @"https://car-rental-api-chezbchwebfggwcd.canadacentral-01.azurewebsites.net";
        private readonly HttpClient _httpClient;
        private string token = "";
        static readonly string login = "JejRental";
        static readonly string password = "q3409tg-hoij";
        //public CarController(ApiContext context)
        //{
        //    _context = context;
        //}

        public CarController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            //GetToken();
        }
        
        public async void GetToken()
        {
            var creds = new LoginToDto() { Username = login, Password = password };

            string json = JsonConvert.SerializeObject(creds);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(forwordURL + "/api/auth/login", httpContent);

            var responseContent = await httpResponse.Content.ReadFromJsonAsync<LoginResponseFromDto>();

            if (responseContent == null)
                return; //StatusCode(500, "bad read");

            token = responseContent!.token!;
        }
        [HttpGet("getAllAvailable")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllAvailableCars()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/user/cars");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpGet("getAllCars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllCars()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/user/cars");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var responseContent = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, responseContent);
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

        [HttpPost("getOffer")]
        public async Task<ActionResult<Offer>> GetOffer([FromBody] OfferRequestFront data)
        {
            var offer = new Offer();

            offer.id = Offer._id;
            Offer._id++;

            offer.carID = data.car!.id;
            decimal dayRate = 10;
            decimal insuranceRate = 1;

            offer.dayRate = dayRate;
            offer.insuranceRate = insuranceRate;
            offer.validUntil = DateTime.Parse(data.enddate);


            return offer;
            /*
            var RentalObj = new OfferRequestBack();

            RentalObj.PlannedStartDate = data.PlannedStartDate;
            RentalObj.PlannedEndDate = data.PlannedEndDate;
            RentalObj.CarId = data.Car!.Id;

            var tmp_content = JsonSerializer.Serialize<OfferRequestBack>(RentalObj);

            var content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(tmp_content),
            Encoding.UTF8,
            "application/json");

            var response = await _httpClient.PostAsync(forwordURL + "/api/user/cars" , content);

            var responseContent = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, responseContent);
            */
        }
        
        [HttpPost("rent")]
        public async Task<ActionResult<Offer>> GetRent([FromBody] RentalOfferFront data)
        {
            
            /*
            var RentalObj = new RentalOfferBack();

            RentalObj.ValidUntil = data.ValidUntil;
            RentalObj.

            var tmp_content = JsonSerializer.Serialize<RentalOfferBack>(RentalObj);

                
            var content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(tmp_content),
            Encoding.UTF8,
            "application/json");
                

            var response = await _httpClient.PostAsync(forwordURL + "/api/user/cars", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, responseContent);
            */
            return Ok("Car rented");
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
