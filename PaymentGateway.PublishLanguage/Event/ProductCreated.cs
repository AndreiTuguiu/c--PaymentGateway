namespace PaymentGateway.PublishedLanguage.Event
{
    public class ProductCreated
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }

        public ProductCreated(string name, double amount, string currency)
        {
            name = Name;
            amount = Amount;
            currency = Currency;
        }
    }
}
