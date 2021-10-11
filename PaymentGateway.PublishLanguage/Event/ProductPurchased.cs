using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Event
{
    public class ProductPurchased
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
