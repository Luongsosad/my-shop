namespace my_shop.Application.DTOs;

public class AppSettingsDto
{
    public string Theme { get; set; } = "System"; // Light, Dark, System
    public int LowStockThreshold { get; set; } = 5;
    public bool NotificationsEnabled { get; set; } = true;
    public bool SoundEffectsEnabled { get; set; } = true;
}

public class UpdateAppSettingsDto
{
    public string Theme { get; set; } = "System";
    public int LowStockThreshold { get; set; } = 5;
    public bool NotificationsEnabled { get; set; } = true;
    public bool SoundEffectsEnabled { get; set; } = true;
}
