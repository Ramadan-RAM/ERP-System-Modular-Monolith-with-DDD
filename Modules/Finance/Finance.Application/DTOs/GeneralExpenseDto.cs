namespace Finance.Application.DTOs
{
    public class GeneralExpenseDto
    {
        public int Id { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public int? CostCenterId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? Description { get; set; }
    }
}
