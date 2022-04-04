using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CreditLine.Model.Entities;
using System.Text.Json;

namespace CreditLine.Infrastructure.Repo
{
    public class DynamoDBRepo
    {

        public async Task<string> ScanCreditLineApplicationResultsAsync()
        {
            using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            DynamoDBContext context = new DynamoDBContext(client);
            Table table = Table.LoadTable(client, "credit-line-application-results");
            var scanOps = new ScanOperationConfig();
            var results = table.Scan(scanOps);
            List<Document> data = await results.GetNextSetAsync();
            IEnumerable<CreditLineApplicationResult> items = context.FromDocuments<CreditLineApplicationResult>(data);
            return JsonSerializer.Serialize(items);
        }

        public async Task<string> WriteCreditLineApplicationResultsAsync(CreditLineApplicationResult creditLineApplicationResult)
        {
            using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            Table table = Table.LoadTable(client, "credit-line-application-results");

            Document item = new Document();
            item["ApplicationId"] = creditLineApplicationResult.ApplicationId;
            item["foundingType"] = creditLineApplicationResult.FoundingType;
            item["cashBalance"] = creditLineApplicationResult.CashBalance;
            item["monthlyRevenue"] = creditLineApplicationResult.MonthlyRevenue;
            item["requestedCreditLine"] = creditLineApplicationResult.RequestedCreditLine;
            item["requestedDate"] = creditLineApplicationResult.RequestedDate;
            item["systemDate"] = DateTime.Now;
            item["isTheCreditLineAccepted"] = creditLineApplicationResult.IsTheCreditLineAccepted;
            item["authorizedCreditLine"] = creditLineApplicationResult.AuthorizedCreditLine;
            item["status"] = creditLineApplicationResult.Status;

            var response = await table.PutItemAsync(item);

            return JsonSerializer.Serialize(item);
        }

        public async Task<string> WriteCreditLineApplicationResultsAsync(Document creditLineApplicationResult)
        {
            using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            Table table = Table.LoadTable(client, "credit-line-application-results");
            creditLineApplicationResult["SystemDate"] = DateTime.Now;
            var response = await table.PutItemAsync(creditLineApplicationResult);
            return JsonSerializer.Serialize(creditLineApplicationResult);
        }

        public List<Document> FindApplicationsByStatus(string status)
        {
            using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            Table table = Table.LoadTable(client, "credit-line-application-results");

            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("status", ScanOperator.Equal, status);

            Search search = table.Scan(scanFilter);
            return search.Matches;                        
        }        
    }
}
