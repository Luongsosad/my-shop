namespace my_shop.Application.DTOs;

public class StoreInfoDto
{
    public string StoreName { get; set; } = "My Shop";
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
}

public class UpdateStoreInfoDto
{
    public string StoreName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
}
