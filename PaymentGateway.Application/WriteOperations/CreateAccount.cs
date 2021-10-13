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
    public class CreateAccount: IWriteOperation<CreateAccountCommand>
    {
        private readonly Database _database;
        private readonly IEventSender _eventSender;
        private readonly AccountOptions _accountOptions;

        public CreateAccount(IEventSender eventSender, AccountOptions accountOptions,Database database)
        {
            _eventSender = eventSender;
            _accountOptions = accountOptions;
            _database = database;
        }

        public void PerformOperation(CreateAccountCommand operation)
        {
            
            var random = new Random();

            Person person;

            if (operation.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.PersonId == operation.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.CNP == operation.Cnp);
            }
            if (person == null)
            {
                throw new Exception("Person not found!");
            }

            
            Account account = new Account();
            account.Balance = _accountOptions.InitialBalance;
            account.Currency = operation.Currency;
            account.IbanCode = operation.IbanCode;

            switch (operation.AccountType)
            {
                case "Current": 
                    account.Type = AccountType.Current;
                    break;
                case "Investment": 
                    account.Type = AccountType.Investment;
                    break;
                case "Economy": 
                    account.Type = AccountType.Economy;
                    break;
                default:
                    Console.WriteLine("Unsupported Account type");
                    break;
            }
            account.Status = AccountStatus.Active;
            account.PersondId = person.PersonId;

            _database.Accounts.Add(account);

            _database.SaveChanges();

            AccountCreated accountCreated = new(operation.IbanCode, operation.AccountType);
            _eventSender.SendEvent(accountCreated);
            
            

        }
    }
}
