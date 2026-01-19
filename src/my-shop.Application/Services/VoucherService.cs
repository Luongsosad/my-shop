using my_shop.Application.DTOs;
using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;
using my_shop.Domain.Exceptions;
using my_shop.Domain.Interfaces;

namespace my_shop.Application.Services;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;

    public VoucherService(IVoucherRepository voucherRepository)
    {
        _voucherRepository = voucherRepository;
    }

    public async Task<IEnumerable<VoucherDto>> GetAllVouchersAsync()
    {
        var vouchers = await _voucherRepository.GetAllAsync();
        return vouchers.Select(MapToDto);
    }

    public async Task<VoucherDto?> GetVoucherByIdAsync(int id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);
        return voucher == null ? null : MapToDto(voucher);
    }

    public async Task<VoucherDto?> GetVoucherByCodeAsync(string code)
    {
        var voucher = await _voucherRepository.GetByCodeAsync(code);
        return voucher == null ? null : MapToDto(voucher);
    }

    public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new DomainException("Mã voucher không được để trống");

        if (dto.DiscountPercent <= 0 || dto.DiscountPercent > 100)
            throw new DomainException("Phần trăm giảm giá phải từ 1-100");

        if (dto.MaxDiscountAmount <= 0)
            throw new DomainException("Số tiền giảm tối đa phải lớn hơn 0");

        if (dto.MinOrderAmount < 0)
            throw new DomainException("Số tiền đơn hàng tối thiểu không được âm");

        if (dto.TotalQuantity <= 0)
            throw new DomainException("Số lượng phát hành phải lớn hơn 0");

        if (dto.EndDate <= dto.StartDate)
            throw new DomainException("Ngày kết thúc phải sau ngày bắt đầu");

        if (await _voucherRepository.ExistsAsync(dto.Code))
            throw new DomainException($"Mã voucher '{dto.Code}' đã tồn tại");

        try
        {
            var voucher = new Voucher
            {
                Code = dto.Code.Trim().ToUpper(),
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                MinOrderAmount = dto.MinOrderAmount,
                TotalQuantity = dto.TotalQuantity,
                RemainingQuantity = dto.TotalQuantity,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _voucherRepository.CreateAsync(voucher);
            return MapToDto(created);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_vouchers_code") == true)
        {
            throw new DomainException($"Mã voucher '{dto.Code}' đã tồn tại");
        }
    }

    public async Task<VoucherDto> UpdateVoucherAsync(UpdateVoucherDto dto)
    {
        var voucher = await _voucherRepository.GetByIdAsync(dto.Id);
        if (voucher == null)
            throw new NotFoundException("Không tìm thấy voucher");

        // Validate
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new DomainException("Mã voucher không được để trống");

        if (dto.DiscountPercent <= 0 || dto.DiscountPercent > 100)
            throw new DomainException("Phần trăm giảm giá phải từ 1-100");

        if (dto.MaxDiscountAmount <= 0)
            throw new DomainException("Số tiền giảm tối đa phải lớn hơn 0");

        if (dto.MinOrderAmount < 0)
            throw new DomainException("Số tiền đơn hàng tối thiểu không được âm");

        if (dto.TotalQuantity <= 0)
            throw new DomainException("Số lượng phát hành phải lớn hơn 0");

        if (dto.RemainingQuantity < 0 || dto.RemainingQuantity > dto.TotalQuantity)
            throw new DomainException("Số lượng còn lại không hợp lệ");

        if (dto.EndDate <= dto.StartDate)
            throw new DomainException("Ngày kết thúc phải sau ngày bắt đầu");

        // Check if code changed and already exists
        if (dto.Code != voucher.Code && await _voucherRepository.ExistsAsync(dto.Code))
            throw new DomainException($"Mã voucher '{dto.Code}' đã tồn tại");

        voucher.Code = dto.Code.Trim().ToUpper();
        voucher.Description = dto.Description;
        voucher.DiscountPercent = dto.DiscountPercent;
        voucher.MaxDiscountAmount = dto.MaxDiscountAmount;
        voucher.MinOrderAmount = dto.MinOrderAmount;
        voucher.TotalQuantity = dto.TotalQuantity;
        voucher.RemainingQuantity = dto.RemainingQuantity;
        voucher.StartDate = dto.StartDate;
        voucher.EndDate = dto.EndDate;

        var updated = await _voucherRepository.UpdateAsync(voucher);
        return MapToDto(updated);
    }

    public async Task DeleteVoucherAsync(int id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);
        if (voucher == null)
            throw new NotFoundException("Không tìm thấy voucher");

        await _voucherRepository.DeleteAsync(id);
    }

    public async Task<VoucherDiscountResult> ApplyVoucherAsync(ApplyVoucherDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            return new VoucherDiscountResult(
                Success: false,
                Message: "Vui lòng nhập mã voucher",
                VoucherId: null,
                DiscountAmount: 0,
                FinalAmount: dto.OrderAmount
            );
        }

        var voucher = await _voucherRepository.GetByCodeAsync(dto.Code.Trim().ToUpper());
        
        if (voucher == null)
        {
            return new VoucherDiscountResult(
                Success: false,
                Message: "Mã voucher không tồn tại",
                VoucherId: null,
                DiscountAmount: 0,
                FinalAmount: dto.OrderAmount
            );
        }

        var now = DateTime.UtcNow;

        // Check if voucher is valid
        if (now < voucher.StartDate)
        {
            return new VoucherDiscountResult(
                Success: false,
                Message: $"Voucher chưa có hiệu lực (bắt đầu: {voucher.StartDate:dd/MM/yyyy})",
                VoucherId: null,
                DiscountAmount: 0,
                FinalAmount: dto.OrderAmount
            );
        }

        if (now > voucher.EndDate)
        {
            return new VoucherDiscountResult(
                Success: false,
                Message: "Voucher đã hết hạn",
                VoucherId: null,
                DiscountAmount: 0,
                FinalAmount: dto.OrderAmount
            );
        }

        if (voucher.RemainingQuantity <= 0)
        {
            return new VoucherDiscountResult(
                Success: false,
                Message: "Voucher đã hết lượt sử dụng",
                VoucherId: null,
                DiscountAmount: 0,
                FinalAmount: dto.OrderAmount
            );
        }

        if (dto.OrderAmount < voucher.MinOrderAmount)
        {
            return new VoucherDiscountResult(
                Success: false,
                Message: $"Đơn hàng tối thiểu {voucher.MinOrderAmount:N0}đ để sử dụng voucher này",
                VoucherId: null,
                DiscountAmount: 0,
                FinalAmount: dto.OrderAmount
            );
        }

        // Calculate discount
        var discountAmount = (dto.OrderAmount * voucher.DiscountPercent) / 100;
        if (discountAmount > voucher.MaxDiscountAmount)
        {
            discountAmount = voucher.MaxDiscountAmount;
        }

        var finalAmount = dto.OrderAmount - discountAmount;

        return new VoucherDiscountResult(
            Success: true,
            Message: $"Giảm {voucher.DiscountPercent}% (tối đa {voucher.MaxDiscountAmount:N0}đ)",
            VoucherId: voucher.Id,
            DiscountAmount: discountAmount,
            FinalAmount: finalAmount
        );
    }

    public async Task<bool> UseVoucherAsync(int voucherId)
    {
        var voucher = await _voucherRepository.GetByIdAsync(voucherId);
        if (voucher == null || voucher.RemainingQuantity <= 0)
            return false;

        voucher.RemainingQuantity--;
        await _voucherRepository.UpdateAsync(voucher);
        return true;
    }

    private static VoucherDto MapToDto(Voucher voucher)
    {
        return new VoucherDto(
            Id: voucher.Id,
            Code: voucher.Code,
            Description: voucher.Description,
            DiscountPercent: voucher.DiscountPercent,
            MaxDiscountAmount: voucher.MaxDiscountAmount,
            MinOrderAmount: voucher.MinOrderAmount,
            TotalQuantity: voucher.TotalQuantity,
            RemainingQuantity: voucher.RemainingQuantity,
            StartDate: voucher.StartDate,
            EndDate: voucher.EndDate,
            IsDeleted: voucher.IsDeleted,
            CreatedAt: voucher.CreatedAt
        );
    }
}
