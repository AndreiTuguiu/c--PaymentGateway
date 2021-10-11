﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class CreateAccountCommand
    {
        public int? PersonId { get; set; }
        public string IbanCode { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public string AccountType { get; set; }
        public string Cnp { get; set; }
    }
}
