using System;

namespace PaymentGateway.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int PersondId { get; set; }
        public string IbanCode { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public AccountType Type { get; set; }
        public AccountStatus Status { get; set; }
        public double Limit { get; set; }
    }
}
