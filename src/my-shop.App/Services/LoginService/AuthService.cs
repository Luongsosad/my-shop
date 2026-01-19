using my_shop.Application.DTOs;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace my_shop.App.Services.LoginService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ITokenService _tokenService;
        private int? _currentAdminId;
        private string? _currentAdminEmail;
        private string? _currentAdminName;

        public AuthService(HttpClient http, ITokenService tokenService)
        {
            _http = http;
            _tokenService = tokenService;

            var instanceId = this.GetHashCode();
            Debug.WriteLine($"🔵 [AuthService Constructor] Instance created: {instanceId}");
        }

        // ==================== PUBLIC PROPERTIES ====================
        public int CurrentAdminId
        {
            get
            {
                if (!_currentAdminId.HasValue)
                {
                    Debug.WriteLine("WARNING: No admin logged in!");
                    throw new InvalidOperationException("No admin logged in");
                }
                return _currentAdminId.Value;
            }
        }

        public string CurrentAdminEmail => _currentAdminEmail ?? throw new InvalidOperationException("No admin logged in");
        public string CurrentAdminName => _currentAdminName ?? "Guest";
        public bool IsLoggedIn => _currentAdminId.HasValue;
        // ===========================================================

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

            // ==================== GET ADMIN INFO ====================
            try
            {
                var adminInfo = await GetAdminByEmailAsync(userEmail!);
                if (adminInfo != null)
                {
                    _currentAdminId = adminInfo.Id;
                    _currentAdminEmail = adminInfo.Email;
                    _currentAdminName = adminInfo.Name ?? "Admin";
                    Debug.WriteLine($"🔵 [LoginAsync] Current admin set: ID={_currentAdminId}, Email={_currentAdminEmail}");
                    Debug.WriteLine($"🔵 [LoginAsync] AuthService instance: {this.GetHashCode()}");
                }
                else
                {
                    Debug.WriteLine("⚠️ Warning: Could not fetch admin details");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Warning: Could not set current admin: {ex.Message}");
            }
            // ========================================================

            Debug.WriteLine($"Login successful for: {userEmail}");

            return new AdminLoginResponseDto
            {
                Email = userEmail!,
                Token = token
            };
        }

        // ==================== INITIALIZE CURRENT ADMIN FROM TOKEN ====================
        public async Task<bool> InitializeCurrentAdminFromTokenAsync()
        {
            try
            {
                Debug.WriteLine("🔵 [InitializeCurrentAdminFromToken] Starting...");

                // Get token to extract email
                var token = await _tokenService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("⚠️ [InitializeCurrentAdminFromToken] No token found");
                    return false;
                }

                // Decode JWT to get email
                var email = ExtractEmailFromToken(token);
                if (string.IsNullOrEmpty(email))
                {
                    Debug.WriteLine("⚠️ [InitializeCurrentAdminFromToken] Could not extract email from token");
                    return false;
                }

                Debug.WriteLine($"🔵 [InitializeCurrentAdminFromToken] Email from token: {email}");

                // Fetch admin details
                var adminInfo = await GetAdminByEmailAsync(email);
                if (adminInfo != null)
                {
                    _currentAdminId = adminInfo.Id;
                    _currentAdminEmail = adminInfo.Email;
                    _currentAdminName = adminInfo.Name ?? "Admin";
                    Debug.WriteLine($"🔵 [InitializeCurrentAdminFromToken] Current admin set: ID={_currentAdminId}, Email={_currentAdminEmail}");
                    Debug.WriteLine($"🔵 [InitializeCurrentAdminFromToken] AuthService instance: {this.GetHashCode()}");
                    return true;
                }

                Debug.WriteLine("⚠️ [InitializeCurrentAdminFromToken] Could not fetch admin details");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [InitializeCurrentAdminFromToken] Error: {ex.Message}");
                Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private string? ExtractEmailFromToken(string token)
        {
            try
            {
                // Simple JWT parsing without library
                var parts = token.Split('.');
                if (parts.Length != 3)
                {
                    Debug.WriteLine("⚠️ [ExtractEmailFromToken] Invalid token format");
                    return null;
                }

                var payload = parts[1];
                // Add padding if needed
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(payload);
                var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

                Debug.WriteLine($"🔵 [ExtractEmailFromToken] Token payload: {json}");

                // Parse JSON to get email
                var jsonDoc = JsonDocument.Parse(json);

                // Try common claim names for email
                if (jsonDoc.RootElement.TryGetProperty("email", out var emailEl))
                {
                    var email = emailEl.GetString();
                    Debug.WriteLine($"🔵 [ExtractEmailFromToken] Found email claim: {email}");
                    return email;
                }
                if (jsonDoc.RootElement.TryGetProperty("sub", out var subEl))
                {
                    var email = subEl.GetString();
                    Debug.WriteLine($"🔵 [ExtractEmailFromToken] Found sub claim: {email}");
                    return email;
                }
                if (jsonDoc.RootElement.TryGetProperty("unique_name", out var nameEl))
                {
                    var email = nameEl.GetString();
                    Debug.WriteLine($"🔵 [ExtractEmailFromToken] Found unique_name claim: {email}");
                    return email;
                }

                Debug.WriteLine("⚠️ [ExtractEmailFromToken] No email claim found in token");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [ExtractEmailFromToken] Error: {ex.Message}");
                return null;
            }
        }
        // =============================================================================

        private async Task<AdminInfoDto?> GetAdminByEmailAsync(string email)
        {
            try
            {
                var res = await _http.PostAsJsonAsync(
                    "/graphql",
                    new
                    {
                        query = @"
                            query GetAdminByEmail($email: String!) {
                                admins(where: { email: { eq: $email } }) {
                                    id
                                    email
                                    name
                                }
                            }
                        ",
                        variables = new { email }
                    });

                var body = await res.Content.ReadAsStringAsync();
                Debug.WriteLine($"🔵 [GetAdminByEmail] Response: {body}");

                var json = JsonDocument.Parse(body);

                if (json.RootElement.TryGetProperty("errors", out var errors))
                {
                    var errorMsg = errors[0].GetProperty("message").GetString();
                    Debug.WriteLine($"❌ [GetAdminByEmail] GraphQL error: {errorMsg}");
                    return null;
                }

                if (json.RootElement.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("admins", out var admins) &&
                    admins.GetArrayLength() > 0)
                {
                    var admin = admins[0];
                    return new AdminInfoDto
                    {
                        Id = admin.GetProperty("id").GetInt32(),
                        Email = admin.GetProperty("email").GetString() ?? "",
                        Name = admin.TryGetProperty("name", out var nameEl) && nameEl.ValueKind != JsonValueKind.Null
                            ? nameEl.GetString() : null
                    };
                }

                Debug.WriteLine("⚠️ [GetAdminByEmail] No admin found");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [GetAdminByEmail] Error: {ex.Message}");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            Debug.WriteLine("🔵 [LogoutAsync] Logging out...");
            await _tokenService.ClearTokenAsync();

            // Clear current admin info
            _currentAdminId = null;
            _currentAdminEmail = null;
            _currentAdminName = null;

            Debug.WriteLine("🔵 [LogoutAsync] Logout completed");
        }
    }

    // Helper DTO
    internal class AdminInfoDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string? Name { get; set; }
    }
}