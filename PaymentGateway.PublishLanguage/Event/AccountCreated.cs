using MediatR;

namespace PaymentGateway.PublishedLanguage.Event
{
    public class AccountCreated :INotification
    {
        public string IbanCode { get; set; }
        public string AccountType { get; set; }

        public AccountCreated(string iban,string type)
        {
            iban = IbanCode;
            type = AccountType;
        }
    }
}
