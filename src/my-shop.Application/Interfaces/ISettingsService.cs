using my_shop.Application.DTOs;

namespace my_shop.Application.Interfaces;

public interface ISettingsService
{
    // Store Information
    Task<StoreInfoDto> GetStoreInfoAsync();
    Task<StoreInfoDto> UpdateStoreInfoAsync(UpdateStoreInfoDto dto);
    
    // User Profile
    Task<UserProfileDto> GetUserProfileAsync(int adminId);
    Task<UserProfileDto> UpdateUserProfileAsync(int adminId, UpdateProfileDto dto);
    Task<bool> ChangePasswordAsync(int adminId, ChangePasswordDto dto);
    
    // App Settings
    Task<AppSettingsDto> GetAppSettingsAsync();
    Task<AppSettingsDto> UpdateAppSettingsAsync(UpdateAppSettingsDto dto);
}
