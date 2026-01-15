using Microsoft.EntityFrameworkCore;
using my_shop.Domain.Entities;
using my_shop.Domain.Interfaces;
using my_shop.Infrastructure.Db;

namespace my_shop.Infrastructure.Repositories;

public class VoucherRepository : IVoucherRepository
{
    private readonly MyShopDbContext _context;

    public VoucherRepository(MyShopDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Voucher>> GetAllAsync()
    {
        return await _context.Vouchers
            .Where(v => !v.IsDeleted)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<Voucher?> GetByIdAsync(int id)
    {
        return await _context.Vouchers
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
    }

    public async Task<Voucher?> GetByCodeAsync(string code)
    {
        return await _context.Vouchers
            .FirstOrDefaultAsync(v => v.Code == code && !v.IsDeleted);
    }

    public async Task<Voucher> CreateAsync(Voucher voucher)
    {
        _context.Vouchers.Add(voucher);
        await _context.SaveChangesAsync();
        return voucher;
    }

    public async Task<Voucher> UpdateAsync(Voucher voucher)
    {
        _context.Vouchers.Update(voucher);
        await _context.SaveChangesAsync();
        return voucher;
    }

    public async Task DeleteAsync(int id)
    {
        var voucher = await GetByIdAsync(id);
        if (voucher != null)
        {
            voucher.IsDeleted = true;
            await UpdateAsync(voucher);
        }
    }

    public async Task<bool> ExistsAsync(string code)
    {
        return await _context.Vouchers
            .AnyAsync(v => v.Code == code && !v.IsDeleted);
    }

    public async Task<bool> CanUseVoucherAsync(string code, int orderAmount)
    {
        var voucher = await GetByCodeAsync(code);
        
        if (voucher == null)
            return false;

        var now = DateTime.UtcNow;
        
        return voucher.RemainingQuantity > 0
            && voucher.StartDate <= now
            && voucher.EndDate >= now
            && orderAmount >= voucher.MinOrderAmount;
    }
}
