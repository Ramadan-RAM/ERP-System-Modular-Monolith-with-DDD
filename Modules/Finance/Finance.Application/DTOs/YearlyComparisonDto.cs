
namespace Finance.Application.DTOs
{
    public class YearlyComparisonDto
    {
        public int Year { get; set; }
        public List<ChartSeriesDto> Series { get; set; } = new();
    }
}
