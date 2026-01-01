using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace my_shop.App.Helpers;

public interface IJwtHelper
{
    JwtPayload? DecodeToken(string token);
    bool IsTokenExpired(string token);
    string? GetClaim(string token, string claimType);
}

public class JwtPayload
{
    public string? Sub { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? AdminId { get; set; }
    public long Exp { get; set; }

    public DateTime ExpirationDateTime => DateTimeOffset.FromUnixTimeSeconds(Exp).DateTime;
    public bool IsExpired => DateTime.UtcNow >= ExpirationDateTime;
    public TimeSpan TimeUntilExpiration => ExpirationDateTime - DateTime.UtcNow;
}

public class JwtHelper : IJwtHelper
{
    /// <summary>
    /// Decode JWT token 
    /// </summary>
    public JwtPayload? DecodeToken(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Debug.WriteLine("❌ Token is null or empty");
                return null;
            }

            // JWT format: HEADER.PAYLOAD.SIGNATURE
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                Debug.WriteLine($"❌ Invalid token format. Expected 3 parts, got {parts.Length}");
                return null;
            }

            // Decode payload (Base64 URL decode)
            var payloadJson = DecodeBase64Url(parts[1]);

            // Parse JSON
            var jsonDoc = JsonDocument.Parse(payloadJson);
            var root = jsonDoc.RootElement;

            var payload = new JwtPayload
            {
                Sub = GetJsonProperty(root, "sub"),
                Email = GetJsonProperty(root, "email"),
                Name = GetJsonProperty(root, "name"),
                Role = GetJsonProperty(root, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"),
                AdminId = GetJsonProperty(root, "admin_id"),
                Exp = GetJsonPropertyLong(root, "exp")
            };

            return payload;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error decoding JWT: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Check if token is expired
    /// </summary>
    public bool IsTokenExpired(string token)
    {
        var payload = DecodeToken(token);
        if (payload == null)
        {
            Debug.WriteLine("❌ Cannot decode token, treating as expired");
            return true;
        }

        if (payload.IsExpired)
        {
            Debug.WriteLine($"❌ Token EXPIRED at {payload.ExpirationDateTime:yyyy-MM-dd HH:mm:ss}");
            return true;
        }

        Debug.WriteLine($"✅ Token valid for {payload.TimeUntilExpiration:hh\\:mm\\:ss} more");
        return false;
    }

    /// <summary>
    /// Get specific claim
    /// </summary>
    public string? GetClaim(string token, string claimType)
    {
        var payload = DecodeToken(token);
        return claimType.ToLower() switch
        {
            "sub" => payload?.Sub,
            "email" => payload?.Email,
            "name" => payload?.Name,
            "role" => payload?.Role,
            "admin_id" => payload?.AdminId,
            _ => null
        };
    }

    private string DecodeBase64Url(string base64Url)
    {
        var padded = base64Url.Length % 4 == 0
            ? base64Url
            : base64Url.PadRight(base64Url.Length + (4 - base64Url.Length % 4), '=');

        padded = padded.Replace('-', '+').Replace('_', '/');
        var bytes = Convert.FromBase64String(padded);
        return Encoding.UTF8.GetString(bytes);
    }

    private string? GetJsonProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private long GetJsonPropertyLong(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number
            ? value.GetInt64()
            : 0;
    }
}
