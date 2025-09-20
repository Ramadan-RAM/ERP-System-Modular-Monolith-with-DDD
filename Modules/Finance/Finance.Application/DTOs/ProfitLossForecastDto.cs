using Finance.Domain.Common.Enums;

namespace Finance.Application.DTOs
{
    public class ProfitLossForecastDto
    {
        public int Id { get; set; }

   
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        // ValueObject
        public MoneyDto TotalExpectedProfit { get; set; } = null!;
        public MoneyDto TotalExpectedLoss { get; set; } = null!;

        //Enum
        public ProfitLossStatus Status { get; set; }
    }
}
