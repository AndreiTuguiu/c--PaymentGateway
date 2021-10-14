using MediatR;

namespace PaymentGateway.PublishedLanguage.Event
{
    public class ProductPurchased : INotification
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }

        public ProductPurchased(int id, int prod, int q, double amount)
        {
            id = AccountId;
            prod = ProductId;
            q = Quantity;
            amount = Amount;
        }
    }
}
