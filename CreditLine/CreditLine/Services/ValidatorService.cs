using CreditLine.Model.DTO;

namespace CreditLine.Services
{
    public class ValidatorService
    {
        public bool ValidateCreditLineInput(CreditLineInput creditLineInput)
        {
            if(creditLineInput.FoundingType != null && creditLineInput.CashBalance != null &&
                creditLineInput.MonthlyRevenue != null && creditLineInput.RequestedCreditLine != null &&
                creditLineInput.RequestedDate != null)
            {
                return true;
            }
            return false;
        }
    }
}
