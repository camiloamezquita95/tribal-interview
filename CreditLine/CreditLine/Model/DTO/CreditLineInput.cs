namespace CreditLine.Model.DTO
{
    public class CreditLineInput
    {
        public const string SME = "SME";
        public const string Startup = "Startup";

        public string? FoundingType { get; set; }
        public decimal? CashBalance { get; set; }
        public decimal? MonthlyRevenue { get; set; }
        public decimal? RequestedCreditLine { get; set; }
        public DateTime? RequestedDate { get; set; }
    }
}
