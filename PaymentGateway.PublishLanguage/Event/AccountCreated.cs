using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Event
{
    public class AccountCreated
    {
        public string IbanCode { get; set; }
        public string AccountType { get; set; }

        public AccountCreated(string iban,string type)
        {
            iban = IbanCode;
            type = AccountType;
        }
    }
}
