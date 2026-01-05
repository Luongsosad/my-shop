using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.Application.Services;

public class AdminService : IAdminService
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AdminService(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<Admin> CreateAsync(string email, string password, string name)
    {
        if (_context.Admins.Any(a => a.Email == email))
            throw new Exception("Email already exists");

        var admin = new Admin
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Name = name,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.Admins.Add(admin);
        Console.WriteLine("Before Save");
        Debug.WriteLine("Before Save");
        var rows = await _context.SaveChangesAsync();
        Console.WriteLine($"Rows affected: {rows}");
        Debug.WriteLine($"Rows affected: {rows}");
        return admin;
    }

    public async Task<string> SessionAsync(string email, string password)
    {
        var admin = _context.Admins.FirstOrDefault(a => a.Email == email);

        if (admin == null)
            throw new Exception("Email not found");

        bool validPassword = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);

        if (!validPassword)
            throw new Exception("Invalid password");

        string token = GenerateJwtToken(admin);
        return token;
    }

    public string GenerateJwtToken(Admin admin)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT Secret Key not configured");
        var issuer = _configuration["JwtSettings:Issuer"] ?? "myshop.api";
        var audience = _configuration["JwtSettings:Audience"] ?? "myshop.client";
        var expirationHours = int.Parse(_configuration["JwtSettings:ExpirationHours"] ?? "24");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, admin.Email),
                new Claim(JwtRegisteredClaimNames.Name, admin.Name ?? admin.Email),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("admin_id", admin.Id.ToString())
            };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        Console.WriteLine($"JWT Token generated for {admin.Email}");
        Debug.WriteLine($"Token: {tokenString}");

        return tokenString;
    }
}
