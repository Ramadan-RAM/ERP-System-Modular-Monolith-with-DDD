
namespace Finance.Application.DTOs
{
    public class ChartSeriesDto
    {
        public string Label { get; set; } = string.Empty; // Month, Category, Supplier
        public decimal Value { get; set; }
    }
}
