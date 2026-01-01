using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;

namespace my_shop.API.GraphQL;

public class Mutation
{
    public async Task<Admin> AdminSignup(
        string email,
        string password,
        string name,
        [Service] IAdminService adminService)
    {
        return await adminService.CreateAsync(email, password, name);
    }
    public async Task<AdminLoginResponse> AdminLogin(
            string email,
            string password,
            [Service] IAdminService adminService)
    {
        var token = await adminService.SessionAsync(email, password);
        return new AdminLoginResponse
        {
            Email = email,
            Token = token
        };
    }
}

public class AdminLoginResponse
{
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}