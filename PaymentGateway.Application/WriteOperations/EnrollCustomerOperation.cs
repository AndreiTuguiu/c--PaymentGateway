using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class EnrollCustomerOperation : IWriteOperation<EnrollCustomerCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly Database _database;
        public EnrollCustomerOperation(IEventSender eventSender,Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }
        public void PerformOperation(EnrollCustomerCommand operation)
        {
            
            Person person = new Person();
            var random = new Random();
            person.CNP = operation.UniqueIdentifier;
            person.Name = operation.Name;
            if (operation.ClientType == "Company")
            {
                person.Type = PersonType.Company;
            }
            else if(operation.ClientType == "Individual")
            {
                person.Type = PersonType.Individual;
            }else
            {
                throw new Exception("Unsupport person type!");
            }


            _database.Persons.Add(person);

            Account account = new Account();
            account.Type = AccountType.Current;
            account.Currency = operation.Currency;
            account.Balance = 0;
            account.IbanCode = random.Next(100000).ToString();

            _database.Accounts.Add(account);

            _database.SaveChanges();
            CustomerEnrolled eventCustEnroll = new(operation.Name,operation.UniqueIdentifier,operation.ClientType);
            eventSender.SendEvent(eventCustEnroll);
        }
    }
}
