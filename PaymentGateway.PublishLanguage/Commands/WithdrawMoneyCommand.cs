using MediatR;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class WithdrawMoneyCommand : IRequest
    {
        public int? AccountId { get; set; }
        public string IbanConde { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}
