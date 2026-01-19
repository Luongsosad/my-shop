using my_shop.Application.DTOs;

namespace my_shop.Application.Interfaces;

public interface IVoucherService
{
    Task<IEnumerable<VoucherDto>> GetAllVouchersAsync();
    Task<VoucherDto?> GetVoucherByIdAsync(int id);
    Task<VoucherDto?> GetVoucherByCodeAsync(string code);
    Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto);
    Task<VoucherDto> UpdateVoucherAsync(UpdateVoucherDto dto);
    Task DeleteVoucherAsync(int id);
    Task<VoucherDiscountResult> ApplyVoucherAsync(ApplyVoucherDto dto);
    Task<bool> UseVoucherAsync(int voucherId);
}
