using my_shop.Application.DTOs;

namespace my_shop.Application.Interfaces;

public interface IReportService
{
    Task<List<ProductSalesReportDto>> GetProductSalesReportAsync(ReportFilterDto filter, int topProducts = 10);
    Task<RevenueProfitReportDto> GetRevenueProfitReportAsync(ReportFilterDto filter);
}
