using Amazon.DynamoDBv2.DocumentModel;
using CreditLine.Infrastructure.Repo;
using CreditLine.Model.Core;
using CreditLine.Model.DTO;
using CreditLine.Model.Entities;
using System.Text.Json;

namespace CreditLine.Services
{
    public class CreditLineService
    {
        private static int CASH_BALANCE_CREDIT_LINE_RATIO = 3;
        private static int MONTHLY_REVENUE_CREDIT_LINE_RATIO = 5;

        private static string ACCEPTED_APPLICATION_STATUS = "A";
        private static string FAILED_APPLICATION_STATUS = "F";

        private readonly DynamoDBRepo dynamoDb = new DynamoDBRepo();

        public CreditLineOutput GetCreditLineResult(CreditLineInput creditLineInput)
        {
            decimal recommendedCreditLine = GetRecommendedCreditLine(creditLineInput);
            bool isAccepted = recommendedCreditLine > creditLineInput.RequestedCreditLine ? true : false;

            CreditLineOutput creditLineResult = new CreditLineOutput()
            {
                IsTheCreditLineAccepted = isAccepted,
                AuthorizedCreditLine = isAccepted? recommendedCreditLine: 0
            };

            return creditLineResult;
        }

        private decimal GetRecommendedCreditLine(CreditLineInput creditLineInput)
        {
            decimal monthlyRevenueCreditLine = creditLineInput.MonthlyRevenue.Value / MONTHLY_REVENUE_CREDIT_LINE_RATIO;

            if (creditLineInput.FoundingType == CreditLineInput.SME)
            {
                return monthlyRevenueCreditLine;
            }
            else if (creditLineInput.FoundingType == CreditLineInput.Startup)
            {
                decimal cashBalanceCreditLine = creditLineInput.CashBalance.Value / CASH_BALANCE_CREDIT_LINE_RATIO;
                return cashBalanceCreditLine > monthlyRevenueCreditLine ? cashBalanceCreditLine : monthlyRevenueCreditLine;
            }
            return 0;
        }

        public async Task<CreditLineApplicationsInfo> ValidatePreviousRequests()
        {
            String previousRequests = await dynamoDb.ScanCreditLineApplicationResultsAsync();
            Console.WriteLine($"Previous Requests JSON: {previousRequests}");
            List<CreditLineApplicationResult>? creditLineApplicationResults = JsonSerializer.Deserialize<List<CreditLineApplicationResult>>(previousRequests);

            CreditLineApplicationsInfo creditLineApplicationsInfo = new CreditLineApplicationsInfo();
            List<Document> acceptedApplications = dynamoDb.FindApplicationsByStatus(ACCEPTED_APPLICATION_STATUS);
            List<Document> failedApplications = dynamoDb.FindApplicationsByStatus(FAILED_APPLICATION_STATUS);

            creditLineApplicationsInfo.AcceptedApplicationExist = acceptedApplications.Count() > 0;
            creditLineApplicationsInfo.AcceptedApplication = acceptedApplications.Count() > 0 ? acceptedApplications[0] : null;
            creditLineApplicationsInfo.FailedRequests = failedApplications.Count;

            if (creditLineApplicationsInfo.AcceptedApplicationExist)
            {
                if (creditLineApplicationResults != null)
                {
                    List<CreditLineApplicationResult> resultsWithin2Minutes = creditLineApplicationResults.Where(c => c.SystemDate >= DateTime.Now.AddMinutes(-2)).ToList();
                    creditLineApplicationsInfo.RequestsWithinTwoMinutes = resultsWithin2Minutes.Count();
                }
            }
            else
            {
                List<CreditLineApplicationResult> resultsWithin30Seconds = creditLineApplicationResults.Where(c => c.SystemDate >= DateTime.Now.AddSeconds(-30)).ToList();
                creditLineApplicationsInfo.RequestsWithin30Seconds = resultsWithin30Seconds.Count();
            }
            return creditLineApplicationsInfo;
        }

        public async Task<string> SaveAcceptedCreditLine(Document acceptedCreditLine)
        {
            String createdItem = await dynamoDb.WriteCreditLineApplicationResultsAsync(acceptedCreditLine);
            return createdItem;
        }

        public async Task<string> SaveCreditLineApplicationResult(CreditLineInput creditLineInput, CreditLineOutput creditLineOutput)
        {
            String previousRequests = await dynamoDb.ScanCreditLineApplicationResultsAsync();
            List<CreditLineApplicationResult>? creditLineApplicationResults = JsonSerializer.Deserialize<List<CreditLineApplicationResult>>(previousRequests);
            int applicationId = 1;
            if(creditLineApplicationResults != null)
            {
                applicationId = creditLineApplicationResults.Count() + 1;
            }

            CreditLineApplicationResult creditLineApplicationResult = new CreditLineApplicationResult()
            {
                ApplicationId = applicationId,
                FoundingType = creditLineInput.FoundingType,
                CashBalance = creditLineInput.CashBalance,
                MonthlyRevenue = creditLineInput.MonthlyRevenue,
                RequestedCreditLine = creditLineInput.RequestedCreditLine,
                RequestedDate = creditLineInput.RequestedDate,
                SystemDate = DateTime.Now,
                IsTheCreditLineAccepted = creditLineOutput.IsTheCreditLineAccepted,
                AuthorizedCreditLine = creditLineOutput.AuthorizedCreditLine,
                Status = creditLineOutput.IsTheCreditLineAccepted ? ACCEPTED_APPLICATION_STATUS : FAILED_APPLICATION_STATUS
            };
            String savedItem = await dynamoDb.WriteCreditLineApplicationResultsAsync(creditLineApplicationResult);
            return savedItem;
        }
    }
}
