namespace PaymentGateway.Models
{
    public class ProductXTransaction
    {
        public int ProductId { get; set; }
        public int TransactionId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public int Quantity { get; set; }
    }
}
