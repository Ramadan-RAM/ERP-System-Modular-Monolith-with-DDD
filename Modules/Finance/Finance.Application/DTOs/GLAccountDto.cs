namespace Finance.Application.DTOs
{
    public class GLAccountDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string BalanceSide { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

}
