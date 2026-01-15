namespace my_shop.Domain.Interfaces;

public interface IVoucherRepository
{
    Task<IEnumerable<Domain.Entities.Voucher>> GetAllAsync();
    Task<Domain.Entities.Voucher?> GetByIdAsync(int id);
    Task<Domain.Entities.Voucher?> GetByCodeAsync(string code);
    Task<Domain.Entities.Voucher> CreateAsync(Domain.Entities.Voucher voucher);
    Task<Domain.Entities.Voucher> UpdateAsync(Domain.Entities.Voucher voucher);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string code);
    Task<bool> CanUseVoucherAsync(string code, int orderAmount);
}
