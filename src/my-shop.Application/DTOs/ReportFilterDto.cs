namespace my_shop.Application.DTOs;

public enum ReportPeriodType
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}

public class ReportFilterDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReportPeriodType PeriodType { get; set; } = ReportPeriodType.Daily;
}
