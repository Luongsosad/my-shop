using Microsoft.EntityFrameworkCore;
using my_shop.Application.DTOs;
using my_shop.Application.Interfaces;
using System.Text.Json;

namespace my_shop.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly IApplicationDbContext _context;
    private const string STORE_INFO_KEY = "store_info";
    private const string APP_SETTINGS_KEY = "app_settings";

    public SettingsService(IApplicationDbContext context)
    {
        _context = context;
    }

    // Store Information
    public async Task<StoreInfoDto> GetStoreInfoAsync()
    {
        // For simplicity, we'll use extension_data in a category or create a settings table
        // For now, return default values (can be enhanced with a Settings table)
        return new StoreInfoDto
        {
            StoreName = "My Shop",
            Address = "123 ???ng ABC, Qu?n 1, TP.HCM",
            Phone = "0123456789",
            Email = "contact@myshop.com",
            TaxCode = "1234567890",
            LogoUrl = ""
        };
    }

    public async Task<StoreInfoDto> UpdateStoreInfoAsync(UpdateStoreInfoDto dto)
    {
        // Store in database (implement when having Settings table)
        // For now, just return the updated values
        return new StoreInfoDto
        {
            StoreName = dto.StoreName,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            TaxCode = dto.TaxCode,
            LogoUrl = dto.LogoUrl
        };
    }

    // User Profile
    public async Task<UserProfileDto> GetUserProfileAsync(int adminId)
    {
        var admin = await _context.Admins.FindAsync(adminId);
        if (admin == null)
            throw new Exception("User not found");

        return new UserProfileDto
        {
            Id = admin.Id,
            Email = admin.Email,
            Name = admin.Name ?? "Admin",
            Theme = admin.Theme ?? "Light",
            IsActive = admin.IsActive,
            CreatedAt = admin.CreatedAt.ToString("dd/MM/yyyy HH:mm")
        };
    }

    public async Task<UserProfileDto> UpdateUserProfileAsync(int adminId, UpdateProfileDto dto)
    {
        var admin = await _context.Admins.FindAsync(adminId);
        if (admin == null)
            throw new Exception("User not found");

        admin.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Theme))
        {
            admin.Theme = dto.Theme;
        }
        admin.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(CancellationToken.None);

        return new UserProfileDto
        {
            Id = admin.Id,
            Email = admin.Email,
            Name = admin.Name ?? "Admin",
            Theme = admin.Theme ?? "Light",
            IsActive = admin.IsActive,
            CreatedAt = admin.CreatedAt.ToString("dd/MM/yyyy HH:mm")
        };
    }

    public async Task<bool> ChangePasswordAsync(int adminId, ChangePasswordDto dto)
    {
        var admin = await _context.Admins.FindAsync(adminId);
        if (admin == null)
            throw new Exception("User not found");

        // Verify current password (use BCrypt in production)
        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, admin.PasswordHash))
            throw new Exception("Current password is incorrect");

        // Hash new password
        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        admin.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(CancellationToken.None);

        return true;
    }

    // App Settings
    public async Task<AppSettingsDto> GetAppSettingsAsync()
    {
        // Return default app settings (can be enhanced with database storage)
        return new AppSettingsDto
        {
            Theme = "System",
            LowStockThreshold = 5,
            NotificationsEnabled = true,
            SoundEffectsEnabled = true
        };
    }

    public async Task<AppSettingsDto> UpdateAppSettingsAsync(UpdateAppSettingsDto dto)
    {
        // Store in local settings (implement persistence later)
        return new AppSettingsDto
        {
            Theme = dto.Theme,
            LowStockThreshold = dto.LowStockThreshold,
            NotificationsEnabled = dto.NotificationsEnabled,
            SoundEffectsEnabled = dto.SoundEffectsEnabled
        };
    }
}
