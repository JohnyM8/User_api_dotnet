using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace WebApplication1.Controllers
{
    /*
    public class GoogleAuthService : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<GoogleTokenResponse> ExchangeCodeForTokens(string code, string redirectUri)
        {
            var tokenRequest = new Dictionary<string, string>
            {
                { "client_id", _configuration["GoogleAuth:ClientId"]! },
                { "client_secret", _configuration["GoogleAuth:ClientSecret"]! },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUri }
            };

            var response = await _httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequest)
            );

            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
            return tokenResponse;
        }

        public async Task<GoogleUserInfo> GetUserInfo(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(
                "https://www.googleapis.com/oauth2/v3/userinfo"
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<GoogleUserInfo>();
        }
    }

    public class GoogleTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? IdToken { get; set; }
    }

    public class GoogleUserInfo
    {
        public string? Sub { get; set; } // Google User ID
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
    */
}
