using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using CreditLine.Model.DTO;
using CreditLine.Services;
using CreditLine.Model.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreditLine;

public class Functions
{

    public static string FAILED_MESSAGE = "A sales agent will contact you";
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions()
    {
    }


    /// <summary>
    /// A Lambda function to process credit line applications
    /// </summary>
    /// <param name="creditLineInput"></param>
    /// <returns>The API Gateway response.</returns>
    public APIGatewayProxyResponse Get(CreditLineInput creditLineInput, ILambdaContext context)
    {
        ValidatorService validatorService = new ValidatorService();

        if(!validatorService.ValidateCreditLineInput(creditLineInput))
        {
            return new APIGatewayProxyResponse()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        context.Logger.LogInformation($"Processing Credit Line Application Founding Type: {creditLineInput.FoundingType} - Requested Credit Line: {creditLineInput.RequestedCreditLine}... \n");

        CreditLineService creditLineService = new CreditLineService();
        CreditLineApplicationsInfo creditLineApplicationsInfo = creditLineService.ValidatePreviousRequests().Result;
        if (creditLineApplicationsInfo.AcceptedApplicationExist)
        {
            if(creditLineApplicationsInfo.RequestsWithinTwoMinutes >= 2)
            {
                return new APIGatewayProxyResponse()
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            else
            {
                string savedItem = creditLineService.SaveAcceptedCreditLine(creditLineApplicationsInfo.AcceptedApplication).Result;
                return new APIGatewayProxyResponse()
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests,
                    Body = savedItem,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }
        else
        {
            if(creditLineApplicationsInfo.RequestsWithin30Seconds >= 1)
            {
                return new APIGatewayProxyResponse()
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            else
            {
                if(creditLineApplicationsInfo.FailedRequests >= 3)
                {
                    return new APIGatewayProxyResponse()
                    {
                        StatusCode = (int)HttpStatusCode.TooManyRequests,
                        Body = FAILED_MESSAGE,
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }
            }
        }


        CreditLineOutput creditLineResult = creditLineService.GetCreditLineResult(creditLineInput);
        string savedItemResult = creditLineService.SaveCreditLineApplicationResult(creditLineInput, creditLineResult).Result;

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize<CreditLineOutput>(creditLineResult),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };

        return response;
    }
}