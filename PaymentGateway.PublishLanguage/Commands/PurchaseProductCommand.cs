using MediatR;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class PurchaseProductCommand : IRequest
    {
        public int? AccountId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool RemoveFromPurchaseIfNotInStock { get; set; }
    }
}
