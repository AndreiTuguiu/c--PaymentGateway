using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentGateway.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            ProductXtransactions = new HashSet<ProductXtransaction>();
        }

        public int TransactionId { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public TransactionType Type { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<ProductXtransaction> ProductXtransactions { get; set; }
    }
}
