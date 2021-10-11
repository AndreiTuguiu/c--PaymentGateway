using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class PurchaseProductCommand
    {
        public int? AccountId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool RemoveFromPurchaseIfNotInStock { get; set; }
    }
}
