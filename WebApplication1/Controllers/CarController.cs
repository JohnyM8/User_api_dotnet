using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Text;
using WebApplication1.Data;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers;

[Route("api/cars")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ApiContext _context;

    static readonly string forwordURL =
        @"http://localhost:5001";

    private readonly HttpClient _httpClient;
    private string? token = null;
    static readonly string login = "JejRental";

    static readonly string password = "q3409tg-hoij";
    //public CarController(ApiContext context)
    //{
    //    _context = context;
    //}

    public CarController(HttpClient httpClient, ApiContext context)
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

        //var responseContent = await httpResponse.Content.ReadAsStringAsync();

        if (responseContent == null)
            return ""; //StatusCode(500, "bad read");

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

        HttpResponseMessage response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        return response;
    }

    [HttpGet("getAllAvailable")]
    public async Task<ActionResult<IEnumerable<Car>>> GetAllAvailableCars()
    {
        //return StatusCode(200 , _context.AllAvailable());

        HttpResponseMessage response = await GetAllAvCars();

        if ((int)response.StatusCode != 200)
            return StatusCode((int)response.StatusCode, null);

        var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

        if (responseContent == null)
            return NotFound();

        var newResponse = responseContent.ConvertToCar();

        //var responseContent = await response.Content.ReadAsStringAsync();

        return Ok(newResponse);
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
        //return StatusCode(200, _context.GetPage(data.Page));
        //var task = Task<ActionResult<IEnumerable<Car>>>.Run(async () => await GetAllAvailableCars());

        //var result = task;
        //int? code = ((IStatusCodeActionResult)task).StatusCode;

        //if (code != 200)
        //    return StatusCode(code.Value, null);

        //var CarList = task.Result.Value;

        HttpResponseMessage response = await GetAllAvCars();

        var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

        if (responseContent == null)
            return NotFound();

        var CarList = responseContent.ConvertToCar();

        if (CarList == null)
            return NotFound("List not found");

        var res = CarList.GetPage(data.Page);

        if (res == null)
            return BadRequest();

        return Ok(res);

    }

    [HttpGet("getCountPages")]
    public async Task<ActionResult<int>> GetCountPage()
    {
        //return StatusCode(200, _context.GetNumPages());

        //var task = Task<ActionResult<IEnumerable<Car>>>.Run(async () => await GetAllAvailableCars());

        //var result = task;
        //int? code = ((IStatusCodeActionResult)task).StatusCode;

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

    [HttpPost("getBathCars")]
    public async Task<ActionResult<IEnumerable<Car>>> GetBathCars([FromBody] BathCarRequest data)
    {


        var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/user/cars");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        var responseContent = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, responseContent);
    }

    [Authorize]
    [HttpPost("getOffer")]
    public async Task<ActionResult<RentalOfferFront>> GetOffer([FromBody] OfferRequestFront data)
    {
        var RentalObj = new OfferRequestDto(data);

        var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals/offers");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(RentalObj);

        HttpResponseMessage response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if ((int)response.StatusCode != 200)
            return StatusCode((int)response.StatusCode, null);
        //if (EmailSender.SendOfferEmail(user.email))
        //    return BadRequest("Email didnt sent");

        var responseContent = await response.Content.ReadFromJsonAsync<RentalOfferDto>();

        var newOffer = new RentalOfferFront(responseContent);

        newOffer.userId = data.CustomerId;

        return StatusCode((int)response.StatusCode, newOffer);
    }

    [Authorize]
    [HttpPost("rent")]
    public async Task<ActionResult<RentalToFront>> GetRent([FromBody] RentalRequestFront data)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var RentalObj = new RentalRequestDto(data);

        request.Content = JsonContent.Create(RentalObj);

        HttpResponseMessage response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if ((int)response.StatusCode != 200)
            return StatusCode((int)response.StatusCode, null);

        if (EmailSender.SendRentEmail(_context.GetUserEmailById(data.CustomerId)))
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

        HttpResponseMessage response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if ((int)response.StatusCode != 200)
            return StatusCode((int)response.StatusCode, null);

        var responseContent = await response.Content.ReadFromJsonAsync<ReturnRecordDto>();

        return StatusCode((int)response.StatusCode, responseContent);
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

            if (car != null && !CarList.Contains(car))
                CarList.Add(car);
        }

        return Ok(CarList);
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCars(int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + $"/api/customer/cars/{id}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        //var responseContent = await response.Content.ReadFromJsonAsync<CarDto>();

        var responseContent = await response.Content.ReadAsStringAsync();

        if (responseContent == null)
            return StatusCode((int)response.StatusCode, responseContent);
        ;

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
            return Ok(years); // Returns a plain JSON array
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