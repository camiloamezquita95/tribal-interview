using CreditLine.Model.Entities;

namespace CreditLine.Model.Core
{
    public class CreditLineApplicationsInfo
    {
        public bool AcceptedApplicationExist { get; set; }
        public CreditLineApplicationResult? AcceptedApplication { get; set; }
        public int RequestsWithinTwoMinutes{ get; set; }
        public int RequestsWithin30Seconds { get; set; }
        public int FailedRequests { get; set; }
    }
}
