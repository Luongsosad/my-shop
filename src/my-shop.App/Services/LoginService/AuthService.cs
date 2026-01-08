using my_shop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace my_shop.App.Services.LoginService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ITokenService _tokenService;

        public AuthService(HttpClient http, ITokenService tokenService)
        {
            _http = http;
            _tokenService = tokenService;
        }

        public async Task<AdminLoginResponseDto> LoginAsync(string email, string password)
        {
            var res = await _http.PostAsJsonAsync(
                "/graphql",
                new
                {
                    query = @"
                mutation ($email: String!, $password: String!) {
                    adminLogin(email: $email, password: $password) {
                        email
                        token
                    }
                }
            ",
                    variables = new { email, password }
                });

            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(body);

            var json = JsonDocument.Parse(body);

            if (json.RootElement.TryGetProperty("errors", out var errors))
            {
                var errorMsg = errors[0].GetProperty("message").GetString();
                throw new Exception($"GraphQL error: {errorMsg}");
            }

            if (!json.RootElement.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("adminLogin", out var loginData))
            {
                throw new Exception("Invalid response format");
            }


            var token = loginData.GetProperty("token").GetString();
            var userEmail = loginData.GetProperty("email").GetString();

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("No token received");
            }

            await _tokenService.SetTokenAsync(token);

            Debug.WriteLine($"Login successful for: {userEmail}");

            return new AdminLoginResponseDto
            {
                Email = userEmail!,
                Token = token
            };
        }

        public async Task LogoutAsync()
        {
            Debug.WriteLine("Logging out...");
            await _tokenService.ClearTokenAsync();
            Debug.WriteLine("Logout completed");
        }
    }
}