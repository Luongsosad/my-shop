using Jose;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using my_shop.API.GraphQL;
using my_shop.Application.Interfaces;
using my_shop.Application.Services;
using my_shop.Infrastructure.Data;
using my_shop.Infrastructure.Db;
using my_shop.Infrastructure.Services;
using System.IO;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var envPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (System.IO.File.Exists(envPath))
{
    foreach (var line in System.IO.File.ReadAllLines(envPath))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;
        var parts = line.Split('=', 2);
        if (parts.Length == 2)
        {
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgresql://"))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    
    if (database.Contains('?'))
    {
        database = database.Substring(0, database.IndexOf('?'));
    }
    
    connectionString = $"Host={uri.Host};Port={port};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Host=localhost;Port=5432;Database=myshop;Username=postgres;Password=postgres";
}

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JWT Secret Key not configured");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "myshop.api";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "myshop.client";

var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<MyShopDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.AddScoped<IApplicationDbContext>(provider => 
    provider.GetRequiredService<MyShopDbContext>());

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddType<DateTimeType>()
    .AddErrorFilter(error =>
    {
        Console.WriteLine($"GraphQL Error: {error.Message}");
        if (error.Exception != null)
        {
            Console.WriteLine($"Exception: {error.Exception}");
        }
        return error;
    });

var app = builder.Build();

try
{
    await DbInitializer.InitializeAsync(app.Services);
}
catch (Exception ex)
{
    Console.WriteLine($"? Failed to initialize database: {ex.Message}");
    return;
}

app.MapGraphQL();
app.Run();
