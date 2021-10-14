using MediatR;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class CreateAccountCommand : IRequest
    {
        public int? PersonId { get; set; }
        public string IbanCode { get; set; }
        public string Currency { get; set; }
        public string AccountType { get; set; }
        public string Cnp { get; set; }
    }
}
