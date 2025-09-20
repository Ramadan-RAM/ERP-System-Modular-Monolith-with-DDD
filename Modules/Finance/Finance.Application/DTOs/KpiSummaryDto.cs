namespace Finance.Application.DTOs
{
    public class KpiSummaryDto
    {
        public decimal NetProfit { get; set; }
        public decimal NetProfitMargin { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal AvgExpensePerMonth { get; set; }
        public decimal YearOverYearGrowth { get; set; } // % مقارنة بالسنة اللي فاتت
    }
}
