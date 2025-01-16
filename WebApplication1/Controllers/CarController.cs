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

        static readonly string forwordURL = "https://car-rental-api-chezbchwebfggwcd.canadacentral-01.azurewebsites.net";
        static readonly string forwordURL2 = "https://rentalapi-esauh2huedhcc2a6.polandcentral-01.azurewebsites.net";
        static readonly string forwordURLLocal = "https://localhost:7151";
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

            var httpContent = new StringContent(
                JsonConvert.SerializeObject(creds),
                Encoding.UTF8,
                "application/json");

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



        public async Task<(IEnumerable<Car>? cars, int statusCode, string? resault)> GetCarsRen2()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL2 + "/api/Vehicle");

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return (null , (int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto2>>();

            if (responseContent == null)
                return (null , NotFound().StatusCode , "Cars not found");

            var newResponse = responseContent.ConvertToCar();

            return (newResponse, Ok().StatusCode, null);
        }

        public async Task<(IEnumerable<Car>? cars, int statusCode, string? resault)> GetCarsRen1()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/customer/cars");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return (null, (int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return (null, NotFound().StatusCode, "Cars not found");

            var newResponse = responseContent.ConvertToCar();

            return (newResponse, Ok().StatusCode, null);
        }

        public async Task<(IEnumerable<Car>? cars, int statusCode, string? resault)> GetAllCars()
        {
            var responce1 = await GetCarsRen1();

            if (responce1.statusCode != 200 || responce1.cars == null)
                return (null , responce1.statusCode, responce1.resault);

            var responce2 = await GetCarsRen2();

            if (responce2.statusCode != 200 || responce2.cars == null)
                return (null , responce2.statusCode, responce2.resault);

            return (responce1.cars.Concat(responce2.cars) , Ok().StatusCode , null);
        }


        [HttpGet("getAllAvailable")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllAvailableCars()
        {

            var responce = await GetAllCars();

            if (responce.statusCode != 200 || responce.cars == null)
                return StatusCode(responce.statusCode , responce.resault);

            return Ok(responce.cars);
        }

        [HttpPost("getPage")]
        public async Task<ActionResult<IEnumerable<Car>>> GetPage([FromBody] PageRequest data)
        {

            var responce = await GetAllCars();

            if (responce.statusCode != 200 || responce.cars == null)
                return StatusCode(responce.statusCode, responce.resault);

            var CarList = responce.cars;    

            var res = CarList.GetPage(data.Page);

            if(res == null)
                return BadRequest();

            return Ok(res);
            
        }

        [HttpGet("getCountPages")]
        public async Task<ActionResult<int>> GetCountPage()
        {
            var responce = await GetAllCars();

            if (responce.statusCode != 200 || responce.cars == null)
                return StatusCode(responce.statusCode, responce.resault);

            var CarList = responce.cars;

            int pages = CarList.CountPages();

            if (pages == -1)
                return NotFound("List not found");
            else if (pages == 0)
                return NotFound("List is empty");
            return Ok(pages);
        }


        public async Task<(RentalOfferFront? content, int statusCode, string? resault)> GetOfferRent1(OfferRequestFront data)
        {
            var RentalObj = new OfferRequestDto(data);

            RentalObj.UpdateUserData(new UserDto(_context.FindUserById(int.Parse(data.CustomerId))));

            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals/offers")
            {
                Content = new StringContent(
                JsonConvert.SerializeObject(RentalObj),
                Encoding.UTF8,
                "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return (null , (int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<RentalOfferDto>();

            if (!EmailSender.SendOfferEmail(_context.GetUserEmailById(int.Parse(data.CustomerId)), responseContent, RentalObj))
                return (null , BadRequest().StatusCode , "Email wasnt send");

            var newOffer = new RentalOfferFront(responseContent);

            newOffer.userId = int.Parse(data.CustomerId);

            return (newOffer , Ok().StatusCode , null);
        }

        public async Task<(RentalOfferFront? content, int statusCode, string? resault)> GetOfferRent2(OfferRequestFront data)
        {
            var RentalObj = new OfferRequestDto(data);

            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL2 + $"/api/Vehicle/available?" +
                $"start={RentalObj.PlannedStartDate.Year}-{RentalObj.PlannedStartDate.Month}-{RentalObj.PlannedStartDate.Day}" +
                $"&end={RentalObj.PlannedEndDate.Year}-{RentalObj.PlannedEndDate.Month}-{RentalObj.PlannedEndDate.Day}");

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return (null, (int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto2>>();

            if (responseContent == null)
                return (null, NotFound().StatusCode, "Cars not found");

            var car = responseContent.ConvertToCar().FindCarById(data.CarId);

            if(car == null)
                return(null, NotFound().StatusCode, "Car is not available at given timespan");

            var user = _context.FindUserById(int.Parse(data.CustomerId));

            if (user == null)
                return (null, NotFound().StatusCode, "User not found");

            RentalOfferDto newOfferDto = new RentalOfferDto()
            {
                Id = RentalOfferDto.index++,
                Car = new CarDto(car),
                DailyRate = Count.CalculateDailyCarRate(new CarDto(car), new UserDto(user)),
                InsuranceRate = Count.CalculateDailyInsuranceRate(new CarDto(car), new UserDto(user)),
                ValidUntil = DateTime.UtcNow.AddMinutes(10),
            };

            if (!EmailSender.SendOfferEmail(_context.GetUserEmailById(int.Parse(data.CustomerId)), newOfferDto, RentalObj))
                return (null, BadRequest().StatusCode, "Email wasnt send");

            var newOffer = new RentalOfferFront(newOfferDto);

            newOffer.userId = int.Parse(data.CustomerId);

            return (newOffer , Ok().StatusCode , null);
        }

        [Authorize]
        [HttpPost("getOffer")]
        public async Task<ActionResult<RentalOfferFront>> GetOffer([FromBody] OfferRequestFront data)
        {
            if (_context.FindUserById(int.Parse(data.CustomerId)) == null)
                return BadRequest("User with given ID does not exists");

            if (data.RentalName == Constants.RentalName)
            {
                var responce1 = await GetOfferRent1(data);

                if (responce1.statusCode != 200 || responce1.content == null)
                    return StatusCode(responce1.statusCode, responce1.resault);

                return Ok(responce1.content);
            }

            else if (data.RentalName == Constants.RentalName2)
            {
                var responce1 = await GetOfferRent2(data);

                if (responce1.statusCode != 200 || responce1.content == null)
                    return StatusCode(responce1.statusCode, responce1.resault);

                return Ok(responce1.content);
            }
            else
                return BadRequest("Wrong rental name");
        }

        public async Task<(RentalDto? data , int statusCode, string? content)> GetRentRental1(RentalRequestFront data)
        {
            var RentalObj = new RentalRequestDto(data);

            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals")
            {
                Content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(RentalObj),
                System.Text.Encoding.UTF8,
                "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);


            if ((int)response.StatusCode != 200)
                return (null , (int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<RentalDto>();

            var CarResponce = await GetCar(responseContent.carId);

            if (CarResponce.statusCode != 200 || CarResponce.car == null)
                return (null, CarResponce.statusCode, "Car was not found\n" + CarResponce.content);

            if (!EmailSender.SendRentEmail(_context.GetUserEmailById(int.Parse(data.CustomerId)) , responseContent , CarResponce.car))
                return (null , BadRequest().StatusCode , "Email didnt sent");

            return (responseContent, Ok().StatusCode , "Rental succesful!\n");
        }

        public async Task<(RentalDto? data, int statusCode, string? content)> GetRentRental2(RentalRequestFront data)
        {
            if (_context.FindUserById(int.Parse(data.CustomerId)) == null)
                return (null , BadRequest().StatusCode , "User with given ID does not exists");

            return (null, NotFound().StatusCode, "Work in progress");
        }


        [HttpGet("rentlink/{Offerid}/{Userid}/{PlannedStartDate}/{PlannedEndDate}/{RentalName}")]
        public async Task<ActionResult<RentalRequestDto>> GetRent(int Offerid , int Userid , string PlannedStartDate , string PlannedEndDate , string RentalName)
        {
            if (RentalName == Constants.RentalName)
            {
                RentalRequestFront data = new RentalRequestFront()
                {
                    OfferId = $"{Offerid}",
                    CustomerId = $"{Userid}",
                    RentalName = Constants.RentalName,
                    PlannedStartDate = PlannedStartDate,
                    PlannedEndDate = PlannedEndDate
                };

                var responce1 = await GetRentRental1(data);

                if (responce1.statusCode != 200)
                    return StatusCode(responce1.statusCode, responce1.content);

                return Ok(responce1.content);
            }
            else if (RentalName == Constants.RentalName2)
            {
                RentalRequestFront data = new RentalRequestFront()
                {
                    OfferId = $"{Offerid}",
                    CustomerId = $"{Userid}",
                    RentalName = Constants.RentalNameTo2,
                    PlannedStartDate = PlannedStartDate,
                    PlannedEndDate = PlannedEndDate
                };

                var responce = await GetRentRental2(data);

                if (responce.statusCode != 200)
                    return StatusCode(responce.statusCode, responce.content);

                return Ok(responce.content);
            }
            else
                return BadRequest("Wrong rental name");
        }

        [Authorize]
        [HttpPost("rent")]
        public async Task<ActionResult<RentalToFront>> GetRent([FromBody] RentalRequestFront data)
        {
            if (data.RentalName == Constants.RentalName)
            {
                var responce1 = await GetRentRental1(data);

                if (responce1.statusCode != 200)
                    return StatusCode(responce1.statusCode, responce1.content);

                return Ok(new RentalToFront(responce1.data));
            }
            else if (data.RentalName == Constants.RentalName2)
            {
                var responce1 = await GetRentRental2(data);

                if (responce1.statusCode != 200)
                    return StatusCode(responce1.statusCode, responce1.content);

                return Ok(new RentalToFront(responce1.data));
            }
            else
                return BadRequest("Wrong rental name");
        }

        public async Task<(ReturnRecordDto? data, int statusCode , string? content)> ReturnCarRen1( ReturnRequestFront data)
        {
            var RentalObj = new ReturnRequestDto(data);

            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals/return")
            {
                Content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(RentalObj),
                System.Text.Encoding.UTF8,
                "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return (null , (int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<ReturnRecordDto>();

            if (!EmailSender.SendReturnStartEmail(_context.GetUserEmailById(int.Parse(data.UserId)) , responseContent))
                return (null , BadRequest().StatusCode , "Email was not send");

            return (responseContent ,  Ok().StatusCode, null);
        }

        public async Task<(ReturnRecordDto? data, int statusCode, string? content)> ReturnCarRen2(ReturnRequestFront data)
        {
            return (null, NotFound().StatusCode, "Work in progress");
        }

        [Authorize]
        [HttpPost("return")]
        public async Task<ActionResult<ReturnRecordDto>> ReturnCar([FromBody] ReturnRequestFront data)
        {
            if(data.RentalName == Constants.RentalName)
            {
                var responce = await ReturnCarRen1(data);

                if (responce.statusCode != 200)
                    return StatusCode(responce.statusCode, responce.content);

                return Ok(responce.data);
            }
            else if(data.RentalName == Constants.RentalName2)
            {
                var responce = await ReturnCarRen2(data);

                if (responce.statusCode != 200)
                    return StatusCode(responce.statusCode, responce.content);

                return Ok(responce.data);
            }
            else
                return BadRequest("Wrong rental name");
        }

        [HttpPost("rentedCars")]
        public async Task<ActionResult<IEnumerable<RentalToFront>>> GetRentedCars([FromBody] RentedCarsRequest data)
        {
            if (_context.FindUserById(int.Parse(data.UserId)) == null)
                return BadRequest("User with given ID does not exists");

            var RequestObj = new RentedCarsRequestDto(data);

            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/customer/rentals/my")
            {
                Content = new StringContent(
               Newtonsoft.Json.JsonConvert.SerializeObject(RequestObj),
               System.Text.Encoding.UTF8,
               "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<RentalDto>>();

            if (responseContent == null)
                return NotFound();

            if (responseContent.Count() == 0)
                return NotFound("User has not rented any cars yet");

            var RentalsList = new List<RentalToFront>();

            foreach (var item in responseContent)
            {
                var Car = await GetCar(item.carId);

                if (Car.car == null)
                    return NotFound("Techical problems: Car with that id was not found");

                var tmp = new RentalToFront(item);

                tmp.Car = Car.car;

                RentalsList.Add(tmp);
            }

            return Ok(RentalsList);
        }

        public async Task<(Car? car, int statusCode , string? content)> GetCar(int Id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + $"/api/customer/cars/{Id}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return (null , (int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<CarDto>();

            if (responseContent == null)
                return (null , NotFound().StatusCode , "Car not found");

            var newCar = new Car(responseContent);

            return (newCar , Ok().StatusCode , null);
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

            return Ok(newCar);
        }


        [HttpGet("distinctBrands")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctBrands()
        {
            var responce = await GetAllCars();

            if (responce.statusCode != 200 || responce.cars == null)
                return StatusCode(responce.statusCode, responce.resault);

            var AllCars = responce.cars;

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
            var responce = await GetAllCars();

            if (responce.statusCode != 200 || responce.cars == null)
                return StatusCode(responce.statusCode, responce.resault);

            var AllCars = responce.cars;

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
            var responce = await GetAllCars();

            if (responce.statusCode != 200 || responce.cars == null)
                return StatusCode(responce.statusCode, responce.resault);

            var AllCars = responce.cars;

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
            var responce = await GetAllCars();

            if (responce.statusCode != 200 || responce.cars == null)
                return StatusCode(responce.statusCode, responce.resault);

            var AllCars = responce.cars;

            var types = AllCars
                .Select(car => car.type)
                .Distinct()
                .ToList();
            return Ok(types); // Returns a plain JSON array
        }

        [HttpGet("distinctLocations")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctLocations()
        {
            var responce = await GetAllCars();

            if (responce.statusCode != 200 || responce.cars == null)
                return StatusCode(responce.statusCode, responce.resault);

            var AllCars = responce.cars;

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
                var responce = await GetAllCars();

                if (responce.statusCode != 200 || responce.cars == null)
                    return StatusCode(responce.statusCode, responce.resault);

                var AllCars = responce.cars;

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


        [HttpPost("return/confirmation")]
        public async Task<ActionResult<string>> ConfReturn([FromBody] ReturnConfReq data)
        {
            if (!EmailSender.SendReturnEndEmail(_context.GetUserEmailById(data.UserId) , data))
                return BadRequest("Email didnt sent");

            return Ok();
        }
















        //GARBAGE ONLY BELOW//





        public async Task<HttpResponseMessage> GetAllAvCars2()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL2 + "/api/Vehicle");

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            return response;
        }


        public async Task<HttpResponseMessage> GetAllAvCars()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, forwordURL + "/api/customer/cars");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            return response;
        }

        [HttpGet("getAllAvailable2")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllAvailableCars2()
        {
            HttpResponseMessage response2 = await GetAllAvCars2();

            if ((int)response2.StatusCode != 200)
                return StatusCode((int)response2.StatusCode, response2.Content.ReadAsStringAsync());

            var responseContent2 = await response2.Content.ReadFromJsonAsync<IEnumerable<CarDto2>>();

            if (responseContent2 == null)
                return NotFound();

            var newResponse2 = responseContent2.ConvertToCar();

            return Ok(newResponse2);
        }




        [HttpGet("getAllAvailableOLD")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllAvailableCarsOLD()
        {

            HttpResponseMessage response = await GetAllAvCars();

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<CarDto>>();

            if (responseContent == null)
                return NotFound();

            var newResponse = responseContent.ConvertToCar();

            HttpResponseMessage response2 = await GetAllAvCars2();

            if ((int)response2.StatusCode != 200)
                return StatusCode((int)response2.StatusCode, response2.Content.ReadAsStringAsync());

            var responseContent2 = await response2.Content.ReadFromJsonAsync<IEnumerable<CarDto2>>();

            if (responseContent2 == null)
                return NotFound();

            var newResponse2 = responseContent2.ConvertToCar();

            return Ok(newResponse.Concat(newResponse2));
        }



        [HttpPost("rentlink")]
        public async Task<ActionResult<string>> GetRentTest([FromBody] RentalRequestFront data)
        {
            var RentalObj = new RentalRequestDto(data);

            var request = new HttpRequestMessage(HttpMethod.Post, forwordURL + "/api/customer/rentals")
            {
                Content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(RentalObj),
                System.Text.Encoding.UTF8,
                "application/json")
            };

            //var request = new HttpRequestMessage(HttpMethod.Post, forwordURLLocal + "/api/rentals")
            //{
            //    Content = new StringContent(
            //    Newtonsoft.Json.JsonConvert.SerializeObject(RentalObj),
            //    System.Text.Encoding.UTF8,
            //    "application/json")
            //};

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            //return Ok(RentalObj);

            //request.Content = JsonContent.Create(RentalObj);

            //request.Content = new StringContent(
            //    Newtonsoft.Json.JsonConvert.SerializeObject(RentalObj));
            //    //System.Text.Encoding.UTF8,
            //    //"application/json");

            //return Ok(request);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if ((int)response.StatusCode != 200)
                return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());

            //if (!EmailSender.SendRentEmail(_context.GetUserEmailById(data.CustomerId)))
            //    return BadRequest("Email didnt sent");

            return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
            //return StatusCode((int)response.StatusCode, "Rental succesful!\n");
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
