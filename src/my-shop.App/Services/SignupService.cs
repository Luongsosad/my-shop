using my_shop.Application.DTOs;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace my_shop.App.Services
{
    public class SignupService : ISignupService
    {
        private readonly HttpClient _http;

        public SignupService(HttpClient http)
        {
            _http = http;
        }

        public async Task SignupAsync(string email, string password, string name)
        {
            var res = await _http.PostAsJsonAsync(
                "/graphql",
                new
                {
                    query = @"
                mutation ($email: String!, $password: String!, $name: String!) {
                    adminSignup(email: $email, password: $password, name: $name) {
                        id
                        email
                        name
                    }
                }
            ",
                    variables = new { email, password, name }
                });

            var body = await res.Content.ReadAsStringAsync();
            
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP {res.StatusCode}: {body}");
            }

            try
            {
                var jsonDoc = JsonDocument.Parse(body);
                
                if (jsonDoc.RootElement.TryGetProperty("errors", out var errors))
                {
                    var errorMessage = errors[0].GetProperty("message").GetString();
                    throw new Exception(errorMessage ?? "Signup failed");
                }
                
                if (!jsonDoc.RootElement.TryGetProperty("data", out var data) ||
                    !data.TryGetProperty("adminSignup", out var adminSignup))
                {
                    throw new Exception("Signup failed: No response data");
                }
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse response: {ex.Message}");
            }
        }
    }
}
