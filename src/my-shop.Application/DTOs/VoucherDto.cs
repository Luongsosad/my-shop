namespace my_shop.Application.DTOs;

public record VoucherDto(
    int Id,
    string Code,
    string? Description,
    int DiscountPercent,
    int MaxDiscountAmount,
    int MinOrderAmount,
    int TotalQuantity,
    int RemainingQuantity,
    DateTime StartDate,
    DateTime EndDate,
    bool IsDeleted,
    DateTime CreatedAt
);

public record CreateVoucherDto(
    string Code,
    string? Description,
    int DiscountPercent,
    int MaxDiscountAmount,
    int MinOrderAmount,
    int TotalQuantity,
    DateTime StartDate,
    DateTime EndDate
);

public record UpdateVoucherDto(
    int Id,
    string Code,
    string? Description,
    int DiscountPercent,
    int MaxDiscountAmount,
    int MinOrderAmount,
    int TotalQuantity,
    int RemainingQuantity,
    DateTime StartDate,
    DateTime EndDate
);

public record ApplyVoucherDto(
    string Code,
    int OrderAmount
);

public record VoucherDiscountResult(
    bool Success,
    string? Message,
    int? VoucherId,
    int DiscountAmount,
    int FinalAmount
);
