using Newtonsoft.Json;

namespace my_shop.App.Models
{
    public class StoreInfoModel
    {
        [JsonProperty("storeName")]
        public string StoreName { get; set; } = "My Shop";

        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty;

        [JsonProperty("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("taxCode")]
        public string TaxCode { get; set; } = string.Empty;

        [JsonProperty("logoUrl")]
        public string LogoUrl { get; set; } = string.Empty;
    }

    public class UserProfileModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("theme")]
        public string Theme { get; set; } = "Light";

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class AppSettingsModel
    {
        [JsonProperty("theme")]
        public string Theme { get; set; } = "System";

        [JsonProperty("lowStockThreshold")]
        public int LowStockThreshold { get; set; } = 5;

        [JsonProperty("notificationsEnabled")]
        public bool NotificationsEnabled { get; set; } = true;

        [JsonProperty("soundEffectsEnabled")]
        public bool SoundEffectsEnabled { get; set; } = true;
    }
}
