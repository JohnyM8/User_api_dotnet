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
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
            Task<string> task = Task<string>.Run(async () => await GetToken());
            token = task.Result;
        }
        
        public async Task<string> GetToken()
        {
            var creds = new LoginToDto() { Username = login, Password = password };

            string json = JsonConvert.SerializeObject(creds);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(forwordURL + "/api/auth/login", httpContent);

            if ((int)httpResponse.StatusCode != 200)
                return "";

            var responseContent = await httpResponse.Content.ReadFromJsonAsync<LoginResponseFromDto>();

            if (responseContent == null)
                return "";

            return responseContent.token!;
        }

        [HttpGet("getToken")]
        public async Task<ActionResult<string>> GetTokenEndPoint()
        {

            Task<string> task = Task<string>.Run(async () => await GetToken());
            token = task.Result;

            return Ok(token);
        }

        public async Task<HttpResponseMessage> GetAllAvCars()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/customer/cars");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            return response;
        }

        [HttpGet("getAllAvailable")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllAvailableCars()
        {

            HttpResponseMessage response = await GetAllAvCars();

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, null);

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if(responseContent == null)
                return NotFound();

            var newResponse = responseContent.ConvertToCar();

            return Ok(newResponse);
        }

        [HttpPost("getPage")]
        public async Task<ActionResult<IEnumerable<Car>>> GetPage([FromBody] PageRequest data)
        {

            HttpResponseMessage response = await GetAllAvCars();

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return NotFound();

            var CarList = responseContent.ConvertToCar();

            if (CarList == null)
                return NotFound("List not found");

            var res = CarList.GetPage(data.Page);

            if(res == null)
                return BadRequest();

            return Ok(res);
            
        }

        [HttpGet("getCountPages")]
        public async Task<ActionResult<int>> GetCountPage()
        {


            HttpResponseMessage response = await GetAllAvCars();

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return NotFound();

            var CarList = responseContent.ConvertToCar();

            int pages = CarList.CountPages();

            if (pages == -1)
                return NotFound("List not found");
            else if (pages == 0)
                return NotFound("List is empty");
            return Ok(pages);
        }

        [Authorize]
        [HttpPost("getOffer")]
        public async Task<ActionResult<RentalOfferFront>> GetOffer([FromBody] OfferRequestFront data)
        {
            if (_context.FindUserById(data.CustomerId) == null)
                return BadRequest("User with given ID does not exists");

            var RentalObj = new OfferRequestDto(data);

            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals/offers");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(RentalObj);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, response.Content.ReadFromJsonAsync<string>());

            var responseContent = await response.Content.ReadFromJsonAsync<RentalOfferDto>();

            if (EmailSender.SendOfferEmail(_context.GetUserEmailById(data.CustomerId) , responseContent , RentalObj))
                return BadRequest("Email wasnt send");

            var newOffer = new RentalOfferFront(responseContent);

            newOffer.userId = data.CustomerId;

            return StatusCode((int)response.StatusCode, newOffer);
        }

        [HttpGet("rentlink/{Offerid}/{Userid}/{PlannedStartDate}/{PlannedEndDate}")]
        public async Task<ActionResult<RentalRequestDto>> GetRent(int Offerid , int Userid , string PlannedStartDate , string PlannedEndDate)
        {
            RentalRequestFront data = new RentalRequestFront() 
            {
                OfferId = Offerid,
                CustomerId = Userid,
                PlannedStartDate = PlannedStartDate,
                PlannedEndDate = PlannedEndDate
            };

            

            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var RentalObj = new RentalRequestDto(data);

            //return Ok(RentalObj);

            request.Content = JsonContent.Create(RentalObj);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, response.Content.ReadFromJsonAsync<string>());

            if (EmailSender.SendRentEmail(_context.GetUserEmailById(data.CustomerId)))
                return BadRequest("Email didnt sent");

            return StatusCode((int)response.StatusCode, "Rental succesful!\n");
        }

        //[Authorize]
        [HttpPost("rent")]
        public async Task<ActionResult<RentalToFront>> GetRent([FromBody] RentalRequestFront data)
        {
            if (_context.FindUserById(data.CustomerId) == null)
                return BadRequest("User with given ID does not exists");

            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var RentalObj = new RentalRequestDto(data);

            request.Content = JsonContent.Create(RentalObj);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, response.Content.ReadFromJsonAsync<string>());

            if(EmailSender.SendRentEmail(_context.GetUserEmailById(data.CustomerId)))
                return BadRequest("Email didnt sent");

            var responseContent = await response.Content.ReadFromJsonAsync<RentalDto>();

            return StatusCode((int)response.StatusCode, new RentalToFront(responseContent));
        }

        [Authorize]
        [HttpPost("return")]
        public async Task<ActionResult<ReturnRecordDto>> ReturnCar([FromBody] ReturnRequestFront data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals/return");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var RentalObj = new ReturnRequestDto(data);

            request.Content = JsonContent.Create(RentalObj);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, response.Content.ReadFromJsonAsync<string>());

            var responseContent = await response.Content.ReadFromJsonAsync<ReturnRecordDto>();

            return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpPost("rentedCars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetRentedCars([FromBody] RentedCarsRequest data)
        {
            if (_context.FindUserById(data.UserId) == null)
                return BadRequest("User with given ID does not exists");

            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/customer/rentals/my");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var RequestObj = new RentedCarsRequestDto(data);

            request.Content = JsonContent.Create(RequestObj);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, null);

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<RentalDto>>();

            if (responseContent == null)
                return NotFound();

            if (responseContent.Count() == 0)
                return NotFound("User has not rented any cars yet");

            var RentalsList = new List<RentalToFront>();

            foreach (var item in responseContent)
            {
                var Car = await GetCar(item.CarId);

                if (Car == null)
                    return NotFound("Techical problems: Car with that id was not found");

                var tmp = new RentalToFront(item);

                tmp.Car = Car;

                RentalsList.Add(tmp);
            }

            return Ok(RentalsList);
        }

        public async Task<Car> GetCar(int Id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + $"/api/customer/cars/{Id}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            //var responseContent = await response.Content.ReadFromJsonAsync<CarDto>();

            var responseContent = await response.Content.ReadAsStringAsync();

            if (responseContent == null)
                return null;

            var DtoCar = JsonConvert.DeserializeObject<CarDto>(responseContent);

            var newCar = new Car(DtoCar);

            return newCar;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCars(int id)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + $"/api/customer/cars/{id}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            //var responseContent = await response.Content.ReadFromJsonAsync<CarDto>();

            var responseContent = await response.Content.ReadAsStringAsync();

            if (responseContent == null)
                return StatusCode((int)response.StatusCode, responseContent); ;

            var DtoCar = JsonConvert.DeserializeObject<CarDto>(responseContent);

            if (DtoCar == null)
                return NotFound("Wrong deserialize");

            var newCar = new Car(DtoCar);

            return StatusCode((int)response.StatusCode, newCar);
        }


        [HttpGet("distinctBrands")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctBrands()
        {
            HttpResponseMessage response = await GetAllAvCars();

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, null);

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return NotFound();

            var AllCars = responseContent.ConvertToCar();

            var brands = AllCars
                .AsQueryable()
                .Select(car => car.producer)
                .Distinct()
                .ToList();
            return Ok(brands); // Returns a plain JSON array
        }

        [HttpGet("modelsByBrand/{producer}")]
        public async Task<ActionResult<IEnumerable<string>>> GetModelsByBrand(string producer)
        {
            HttpResponseMessage response = await GetAllAvCars();

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, null);

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return NotFound();

            var AllCars = responseContent.ConvertToCar();

            var models = AllCars
                .Where(car => car.producer == producer)
                .Select(car => car.model)
                .Distinct()
                .ToList();
            return Ok(models); // Returns a plain JSON array
        }

        [HttpGet("distinctYears")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctYears()
        {
            HttpResponseMessage response = await GetAllAvCars();

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, null);

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return NotFound();

            var AllCars = responseContent.ConvertToCar();

            var years = AllCars
                .Select(car => car.yearOfProduction)
                .Distinct()
                .OrderBy(year => year)
                .ToList();
            return Ok(years);
        }

        [HttpGet("distinctTypes")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctTypes()
        {
            HttpResponseMessage response = await GetAllAvCars();

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, null);

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return NotFound();

            var AllCars = responseContent.ConvertToCar();

            var types = AllCars
                .Select(car => car.type)
                .Distinct()
                .ToList();
            return Ok(types); // Returns a plain JSON array
        }

        [HttpGet("distinctLocations")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctLocations()
        {
            HttpResponseMessage response = await GetAllAvCars();

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, null);

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return NotFound();

            var AllCars = responseContent.ConvertToCar();

            var locations = AllCars
                .Select(car => car.location)
                .Distinct()
                .ToList();
            return Ok(locations); // Returns a plain JSON array
        }

        /* FILTERING CARS */
        [HttpGet("filteredCars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetFilteredCars(
            [FromQuery] string? producer,
            [FromQuery] string? model,
            [FromQuery] int? yearOfProduction,
            [FromQuery] string? type,
            [FromQuery] string? location)
        {
            try
            {
                HttpResponseMessage response = await GetAllAvCars();

                if ((int)response.StatusCode != 200)
                    return StatusCode((int)response.StatusCode, null);

                var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

                if (responseContent == null)
                    return NotFound();

                var AllCars = responseContent.ConvertToCar();

                var filteredCars = AllCars.AsQueryable();

                if (!string.IsNullOrEmpty(producer))
                {
                    filteredCars = filteredCars.Where(car => car.producer == producer);
                }

                if (!string.IsNullOrEmpty(model))
                {
                    filteredCars = filteredCars.Where(car => car.model == model);
                }

                if (yearOfProduction.HasValue)
                {
                    // Convert yearOfProduction to a string and compare
                    int year = yearOfProduction.Value;
                    filteredCars = filteredCars.Where(car => car.yearOfProduction == year.ToString());
                }

                if (!string.IsNullOrEmpty(type))
                {
                    filteredCars = filteredCars.Where(car => car.type == type);
                }

                if (!string.IsNullOrEmpty(location))
                {
                    filteredCars = filteredCars.Where(car => car.location == location);
                }

                var result = filteredCars.ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }




        //GARBAGE ONLY BELOW//



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
