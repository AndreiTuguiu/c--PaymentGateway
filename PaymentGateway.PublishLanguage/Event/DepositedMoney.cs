using MediatR;

namespace PaymentGateway.PublishedLanguage.Event
{
    public class DepositedMoney : INotification
    {
        public string IbanCode { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }

        public DepositedMoney(string ibanCode, double amount, string curr)
        {
            ibanCode = IbanCode;
            amount = Amount;
            curr = Currency;
        }
    }

}
