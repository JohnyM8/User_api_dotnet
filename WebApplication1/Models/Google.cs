using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IdentityModel.Tokens.Jwt;
using System.Web;
using System.Drawing;

namespace WebApplication1.Models
{
    public class GoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public string GetJsonString(string code , string redirectUri)
        {
            return "{" + $" \"code\" : \"{code}\" ," +
                $"\"client_id\" : \"{_configuration["jsonCopy:GoogleAuth:ClientId"]!}\"," +
                $"\"client_secret\" : \"{_configuration["jsonCopy:GoogleAuth:ClientSecret"]!}\"," +
                $"\"redirect_uri\": \"{redirectUri}\" ," +
                $"\"grant_type\": \"authorization_code\"" + "}";
        }

        public async Task<GoogleTokenResponse> ExchangeCodeForTokens(string code, string redirectUri)
        {

            string decode_code = HttpUtility.UrlDecode(code);

            //var tokenRequest = new Dictionary<string, string>
            //{
            //    { "client_id", _configuration["jsonCopy:GoogleAuth:ClientId"]! },
            //    { "client_secret", _configuration["jsonCopy:GoogleAuth:ClientSecret"]! },
            //    { "code", decode_code },
            //    { "grant_type", "authorization_code" },
            //    { "redirect_uri", redirectUri }
            //};

            
            
            var jsonRequest = GetJsonString(decode_code , redirectUri);

            var response = await _httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new StringContent(jsonRequest, Encoding.UTF8, "application/json")
            );
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
            return tokenResponse;
        }

        public GoogleUserInfo GetUserInfoFromIdToken(string id_token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(id_token);

            var payload = jsonToken.Payload;

            string email = payload["email"].ToString();
            string firstName = payload["given_name"].ToString();
            string lastName = payload["family_name"].ToString();

            var user = new GoogleUserInfo()
            {
                Email = email,
                Name = firstName,
            };

            return user;
        }
        public async Task<GoogleUserInfo> GetUserInfo(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(
                "https://www.googleapis.com/oauth2/v3/userinfo"
            );

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return await response.Content.ReadFromJsonAsync<GoogleUserInfo>();
        }
    }

    public class GoogleTokenResponse
    {
        public string? access_token { get; set; }
        public int expires_in { get; set; }
        public string? scope { get; set; }
        public string? token_type { get; set; }
        public string? id_token { get; set; }
    }

    public class GoogleUserInfo
    {
        public string? Sub { get; set; } // Google User ID
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
    public class ApplicationUser
    {
        public string? Id { get; set; }
        public string? GoogleId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public bool IsProfileCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GoogleAuthRequest
    {
        public string? Code { get; set; }
        public string? RedirectUri { get; set; }
    }

    public class GoogleAuthRegisterRequest
    {
        public string? Email { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public string? login { get; set; }
        public string? rentalService { get; set; }
        public string? birthday { get; set; }
        public string? driverLicenseReceiveDate { get; set; }
    }

    public class AuthRegisterResponse
    {
        public bool? IsProfileComplete { get; set; }
        public User User { get; set; }
    }


    public class AuthResponse
    {
        public string? Token { get; set; }
        public UserDtoGoogle User { get; set; }
        public bool IsNewUser { get; set; }
    }

    public class UserDtoGoogle
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public bool ProfileCompleted { get; set; }

        public UserDtoGoogle(GoogleUserInfo data)
        {
            this.Id = data.Sub;
            this.Email = data.Email;
            this.Name = data.Name;
        }
    }

    public class TokenResetResponse
    {
        public string? Token { get; set; }
    }
}
