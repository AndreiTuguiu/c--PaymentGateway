using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateAccount: IRequestHandler<CreateAccountCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;
        private readonly AccountOptions _accountOptions;

        public CreateAccount(IMediator mediator, AccountOptions accountOptions,Database database)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _database = database;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var random = new Random();

            Person person;

            if (request.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.CNP == request.Cnp);
            }
            if (person == null)
            {
                throw new Exception("Person not found!");
            }


            Account account = new Account();
            account.Balance = _accountOptions.InitialBalance;
            account.Currency = request.Currency;
            account.IbanCode = request.IbanCode;

            switch (request.AccountType)
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

            account.AccountId = _database.Accounts.Count + 1;

            _database.Accounts.Add(account);

            _database.SaveChanges();

            AccountCreated accountCreated = new(request.IbanCode, request.AccountType);
            await _mediator.Publish(accountCreated,cancellationToken);

            return Unit.Value;
        }

        
    }
}
