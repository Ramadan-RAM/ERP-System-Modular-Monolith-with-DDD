namespace Finance.Application.DTOs
{
    public class MoneyDto
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
