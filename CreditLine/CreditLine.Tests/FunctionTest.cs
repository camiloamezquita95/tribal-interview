using Xunit;
using CreditLine.Model.DTO;
using CreditLine.Services;

namespace CreditLine.Tests;

public class FunctionTest
{
    public FunctionTest()
    {
    }

    [Fact]
    public void TestGetRecommendedCreditLineSME()
    {        
        CreditLineInput creditLineInput = new CreditLineInput()
        {
            FoundingType = "SME",
            CashBalance = Convert.ToDecimal(435.30),
            MonthlyRevenue = Convert.ToDecimal(4235.45),
            RequestedCreditLine = 100,
            RequestedDate = Convert.ToDateTime("2021-07-19T16:32:59.860Z")
        };

        CreditLineService creditLineService = new CreditLineService();
        decimal expectedCreditLine = creditLineInput.MonthlyRevenue.Value / 5;
        decimal authorizedCreditLine = creditLineService.GetRecommendedCreditLine(creditLineInput);

        Assert.Equal(expectedCreditLine, authorizedCreditLine);
    }

    [Fact]
    public void TestGetRecommendedCreditLineStartup()
    {
        CreditLineInput creditLineInput = new CreditLineInput()
        {
            FoundingType = "Startup",
            CashBalance = Convert.ToDecimal(435.30),
            MonthlyRevenue = Convert.ToDecimal(4235.45),
            RequestedCreditLine = 100,
            RequestedDate = Convert.ToDateTime("2021-07-19T16:32:59.860Z")
        };

        CreditLineService creditLineService = new CreditLineService();
        decimal cashBalanceCreditLine = creditLineInput.CashBalance.Value / 3;
        decimal monthlyRevenueCreditLine = creditLineInput.MonthlyRevenue.Value / 5;
        decimal expectedCreditLine = cashBalanceCreditLine > monthlyRevenueCreditLine ? cashBalanceCreditLine : monthlyRevenueCreditLine;
        decimal authorizedCreditLine = creditLineService.GetRecommendedCreditLine(creditLineInput);

        Assert.Equal(expectedCreditLine, authorizedCreditLine);
    }

    [Fact]
    public void TestGetCreditLineResultSMEAccepted()
    {
        CreditLineInput creditLineInput = new CreditLineInput()
        {
            FoundingType = "SME",
            CashBalance = Convert.ToDecimal(435.30),
            MonthlyRevenue = Convert.ToDecimal(4235.45),
            RequestedCreditLine = 100,
            RequestedDate = Convert.ToDateTime("2021-07-19T16:32:59.860Z")
        };

        CreditLineService creditLineService = new CreditLineService();
        CreditLineOutput expectedCreditLineOutput = new CreditLineOutput()
        {
            IsTheCreditLineAccepted = true,
            AuthorizedCreditLine = creditLineInput.MonthlyRevenue.Value / 5
        };
        CreditLineOutput creditLineOutput = creditLineService.GetCreditLineResult(creditLineInput);

        Assert.Equal(expectedCreditLineOutput.IsTheCreditLineAccepted, creditLineOutput.IsTheCreditLineAccepted);
        Assert.Equal(expectedCreditLineOutput.AuthorizedCreditLine, creditLineOutput.AuthorizedCreditLine);
    }

    [Fact]
    public void TestGetCreditLineResultStartupAccepted()
    {
        CreditLineInput creditLineInput = new CreditLineInput()
        {
            FoundingType = "Startup",
            CashBalance = Convert.ToDecimal(435.30),
            MonthlyRevenue = Convert.ToDecimal(4235.45),
            RequestedCreditLine = 100,
            RequestedDate = Convert.ToDateTime("2021-07-19T16:32:59.860Z")
        };

        CreditLineService creditLineService = new CreditLineService();
        CreditLineOutput expectedCreditLineOutput = new CreditLineOutput()
        {
            IsTheCreditLineAccepted = true,
        };
        decimal cashBalanceCreditLine = creditLineInput.CashBalance.Value / 3;
        decimal monthlyRevenueCreditLine = creditLineInput.MonthlyRevenue.Value / 5;
        decimal expectedCreditLine = cashBalanceCreditLine > monthlyRevenueCreditLine ? cashBalanceCreditLine : monthlyRevenueCreditLine;
        expectedCreditLineOutput.AuthorizedCreditLine = expectedCreditLine;

        CreditLineOutput creditLineOutput = creditLineService.GetCreditLineResult(creditLineInput);

        Assert.Equal(expectedCreditLineOutput.IsTheCreditLineAccepted, creditLineOutput.IsTheCreditLineAccepted);
        Assert.Equal(expectedCreditLineOutput.AuthorizedCreditLine, creditLineOutput.AuthorizedCreditLine);
    }
}