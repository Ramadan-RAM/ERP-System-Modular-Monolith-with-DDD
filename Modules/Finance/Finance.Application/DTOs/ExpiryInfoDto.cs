namespace Finance.Application.DTOs
{
    public class ExpiryInfoDto
    {
        public DateTime? ExpiryDate { get; set; }

        
        public int? DaysToExpire { get; set; }

        
        public int WarningDays { get; set; }
    }
}
