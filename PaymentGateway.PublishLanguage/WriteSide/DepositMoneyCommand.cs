using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class DepositMoneyCommand
    {
        public int? AccountId { get; set; }
        public string IbanConde { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}
