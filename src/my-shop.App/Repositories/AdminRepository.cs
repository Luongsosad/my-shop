using my_shop.App.Services.LicenseService;
using my_shop.Domain.Entities;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace my_shop.App.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly HttpClient _httpClient;

        public AdminRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            try
            {
                var query = @"
                    query GetAdmin($adminId: Int!) {
                        admin(adminId: $adminId) {
                            id
                            email
                            name
                            isActive
                            createdAt
                            updatedAt
                            trialStartDate
                            isActivated
                            activationCode
                            activatedAt
                        }
                    }";

                var request = new
                {
                    query,
                    variables = new { adminId = id }
                };

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/graphql", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"GraphQL Error: {response.StatusCode} - {errorBody}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"GraphQL Response: {jsonResponse}");

                var jsonDoc = JsonDocument.Parse(jsonResponse);

                // Check for GraphQL errors
                if (jsonDoc.RootElement.TryGetProperty("errors", out var errors))
                {
                    var errorMsg = errors[0].GetProperty("message").GetString();
                    Debug.WriteLine($"GraphQL Error: {errorMsg}");
                    return null;
                }

                // Parse data
                if (!jsonDoc.RootElement.TryGetProperty("data", out var data) ||
                    !data.TryGetProperty("admin", out var adminData))
                {
                    Debug.WriteLine("No admin data in response");
                    return null;
                }

                // Manual mapping from JSON to Admin entity
                var admin = new Admin
                {
                    Id = adminData.GetProperty("id").GetInt32(),
                    Email = adminData.GetProperty("email").GetString() ?? "",
                    Name = adminData.TryGetProperty("name", out var nameEl) && nameEl.ValueKind != JsonValueKind.Null
                        ? nameEl.GetString() : null,
                    IsActive = adminData.GetProperty("isActive").GetBoolean(),
                    CreatedAt = adminData.GetProperty("createdAt").GetDateTimeOffset(),
                    UpdatedAt = adminData.TryGetProperty("updatedAt", out var updatedEl) && updatedEl.ValueKind != JsonValueKind.Null
                        ? updatedEl.GetDateTimeOffset() : null,
                    TrialStartDate = adminData.TryGetProperty("trialStartDate", out var trialEl) && trialEl.ValueKind != JsonValueKind.Null
                        ? trialEl.GetDateTimeOffset() : null,
                    IsActivated = adminData.GetProperty("isActivated").GetBoolean(),
                    ActivationCode = adminData.TryGetProperty("activationCode", out var codeEl) && codeEl.ValueKind != JsonValueKind.Null
                        ? codeEl.GetString() : null,
                    ActivatedAt = adminData.TryGetProperty("activatedAt", out var activatedEl) && activatedEl.ValueKind != JsonValueKind.Null
                        ? activatedEl.GetDateTimeOffset() : null
                };

                return admin;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting admin: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(Admin admin)
        {
            try
            {
                // Use the UpdateAdminLicense mutation from backend
                var mutation = @"
                    mutation UpdateAdminLicense(
                        $adminId: Int!
                        $trialStartDate: DateTime
                        $isActivated: Boolean!
                        $activationCode: String
                        $activatedAt: DateTime
                    ) {
                        updateAdminLicense(
                            adminId: $adminId
                            trialStartDate: $trialStartDate
                            isActivated: $isActivated
                            activationCode: $activationCode
                            activatedAt: $activatedAt
                        ) {
                            id
                            isActivated
                            activationCode
                            activatedAt
                            trialStartDate
                        }
                    }";

                var variables = new
                {
                    adminId = admin.Id,
                    trialStartDate = admin.TrialStartDate,
                    isActivated = admin.IsActivated,
                    activationCode = admin.ActivationCode,
                    activatedAt = admin.ActivatedAt
                };

                var request = new
                {
                    query = mutation,
                    variables
                };

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                Debug.WriteLine($"Update request: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/graphql", content);

                var responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Update response: {responseBody}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to update admin: {response.StatusCode} - {responseBody}");
                }

                var jsonDoc = JsonDocument.Parse(responseBody);
                if (jsonDoc.RootElement.TryGetProperty("errors", out var errors))
                {
                    var errorMsg = errors[0].GetProperty("message").GetString();
                    throw new Exception($"GraphQL Error: {errorMsg}");
                }

                Debug.WriteLine("Admin updated successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating admin: {ex.Message}");
                throw;
            }
        }
    }
}