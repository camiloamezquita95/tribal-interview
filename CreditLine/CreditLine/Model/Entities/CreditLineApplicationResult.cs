namespace CreditLine.Model.Entities
{
    public class CreditLineApplicationResult
    {
        public int ApplicationId { get; set; }
        public string? FoundingType { get; set; }
        public decimal? CashBalance { get; set; }
        public decimal? MonthlyRevenue { get; set; }
        public decimal? RequestedCreditLine { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? SystemDate { get; set; }
        public bool IsTheCreditLineAccepted { get; set; }
        public decimal AuthorizedCreditLine { get; set; }
        public String Status{ get; set; }
    }
}
