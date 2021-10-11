using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string CNP { get; set; }
        public PersonType Type { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}
