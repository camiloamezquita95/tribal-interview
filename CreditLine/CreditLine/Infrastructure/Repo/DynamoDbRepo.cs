using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using CreditLine.Model.Entities;
using System.Text.Json;

namespace CreditLine.Infrastructure.Repo
{
    public class DynamoDBRepo
    {

        public async Task<List<CreditLineApplicationResult>> ScanCreditLineApplicationResultsAsync()
        {
            using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            DynamoDBContext context = new DynamoDBContext(client);
            Table table = Table.LoadTable(client, "credit-line-application-results");
            var scanOps = new ScanOperationConfig();
            var results = table.Scan(scanOps);
            List<Document> data = await results.GetNextSetAsync();
            List<CreditLineApplicationResult> items = context.FromDocuments<CreditLineApplicationResult>(data).ToList();
            return items;
        }

        public async Task<string> WriteCreditLineApplicationResultsAsync(CreditLineApplicationResult creditLineApplicationResult)
        {
            using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            Table table = Table.LoadTable(client, "credit-line-application-results");

            Document item = new Document();
            item["ApplicationId"] = creditLineApplicationResult.ApplicationId;
            item["FoundingType"] = creditLineApplicationResult.FoundingType;
            item["CashBalance"] = creditLineApplicationResult.CashBalance;
            item["MonthlyRevenue"] = creditLineApplicationResult.MonthlyRevenue;
            item["RequestedCreditLine"] = creditLineApplicationResult.RequestedCreditLine;
            item["RequestedDate"] = creditLineApplicationResult.RequestedDate;
            item["SystemDate"] = DateTime.Now;
            item["IsTheCreditLineAccepted"] = creditLineApplicationResult.IsTheCreditLineAccepted;
            item["AuthorizedCreditLine"] = creditLineApplicationResult.AuthorizedCreditLine;
            item["Status"] = creditLineApplicationResult.Status;

            var response = await table.PutItemAsync(item);

            return JsonSerializer.Serialize(creditLineApplicationResult);
        }

        public async Task<string> WriteCreditLineApplicationResultsAsync(Document creditLineApplicationResult)
        {
            using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            Table table = Table.LoadTable(client, "credit-line-application-results");
            creditLineApplicationResult["SystemDate"] = DateTime.Now;
            var response = await table.PutItemAsync(creditLineApplicationResult);
            return JsonSerializer.Serialize(creditLineApplicationResult);
        }

        public List<CreditLineApplicationResult> FindApplicationsByStatus(string status)
        {

            using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            DynamoDBContext context = new DynamoDBContext(client);
            Table table = Table.LoadTable(client, "credit-line-application-results");
            var scanOps = new ScanOperationConfig();
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("Status", ScanOperator.Equal, status);
            var results = table.Scan(scanFilter);
            List<Document> data = results.GetNextSetAsync().Result;
            List<CreditLineApplicationResult> items = context.FromDocuments<CreditLineApplicationResult>(data).ToList();
            return items;
        }

        public async Task<int> GetNextApplicationId()
        {
            List<CreditLineApplicationResult> creditLineApplicationResults = await ScanCreditLineApplicationResultsAsync();
            int applicationId = 1;
            if (creditLineApplicationResults != null)
            {
                applicationId = creditLineApplicationResults.Count() + 1;
            }
            return applicationId;
        }
    }
}
