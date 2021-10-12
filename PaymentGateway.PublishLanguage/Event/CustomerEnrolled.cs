﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Event
{
    public class CustomerEnrolled
    {
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }
        public string ClientType { get; set; }

        public CustomerEnrolled(string name, string uq, string ct)
        {
            name = Name;
            uq = UniqueIdentifier;
            ct = ClientType;
        }
    }
}