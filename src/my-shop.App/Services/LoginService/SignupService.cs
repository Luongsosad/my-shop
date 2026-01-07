using my_shop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static my_shop.Application.DTOs.AdminSignupDto;
using static System.Net.WebRequestMethods;

namespace my_shop.App.Services.LoginService
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
                throw new Exception(body);
        }

    }
}
