using Microsoft.IdentityModel.Tokens;
using my_shop.App.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace my_shop.App.Services;

public interface ITokenService
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task ClearTokenAsync();
    Task<bool> IsTokenValidAsync();
}

public class TokenService : ITokenService
{
    private readonly IJwtHelper _jwtHelper;
    private const string TOKEN_KEY = "auth_token";

    public TokenService(IJwtHelper jwtHelper)
    {
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// Get token from storage
    /// </summary>
    public Task<string?> GetTokenAsync()
    {
        var token = ApplicationData.Current.LocalSettings.Values[TOKEN_KEY] as string;

        if (string.IsNullOrEmpty(token))
        {
            Debug.WriteLine("No token found in storage");
        }
        else
        {
            Debug.WriteLine($"Token retrieved: {token.Substring(0, Math.Min(20, token.Length))}...");
        }

        return Task.FromResult(token);
    }

    /// <summary>
    /// Save token to storage
    /// </summary>
    public Task SetTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            Debug.WriteLine("Cannot save empty token");
            return Task.CompletedTask;
        }

        ApplicationData.Current.LocalSettings.Values[TOKEN_KEY] = token;
        Debug.WriteLine($"Token saved to storage");

        // Log expiration info
        var payload = _jwtHelper.DecodeToken(token);
        if (payload != null)
        {
            Debug.WriteLine($"   Expires at: {payload.ExpirationDateTime:yyyy-MM-dd HH:mm:ss}");
            Debug.WriteLine($"   Valid for: {payload.TimeUntilExpiration:hh\\:mm\\:ss}");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Clear token from storage
    /// </summary>
    public Task ClearTokenAsync()
    {
        ApplicationData.Current.LocalSettings.Values.Remove(TOKEN_KEY);
        Debug.WriteLine("Token cleared from storage");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Check if token exists and is not expired
    /// </summary>
    public async Task<bool> IsTokenValidAsync()
    {
        var token = await GetTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            Debug.WriteLine("No token - not valid");
            return false;
        }

        var payload = _jwtHelper.DecodeToken(token);
        if (payload == null)
        {
            Debug.WriteLine("Cannot decode token - not valid");
            return false;
        }

        if (payload.IsExpired)
        {
            Debug.WriteLine($"Token expired at {payload.ExpirationDateTime:yyyy-MM-dd HH:mm:ss} - not valid");
            return false;
        }

        Debug.WriteLine($"Token is valid, expires in {payload.TimeUntilExpiration:hh\\:mm\\:ss}");
        return true;
    }
}
