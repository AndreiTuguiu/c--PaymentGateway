using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace PaymentGateway.Application.CommandHandlers
{
    public class CreateAccount: IRequestHandler<CreateAccountCommand>
    {
        private readonly PaymentDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly AccountOptions _accountOptions;

        public CreateAccount(IMediator mediator, AccountOptions accountOptions,PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var random = new Random();

            Person person;

            if (request.PersonId.HasValue)
            {
                person = _dbContext.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _dbContext.Persons.FirstOrDefault(x => x.Cnp == request.Cnp);
            }
            if (person == null)
            {
                throw new Exception("Person not found!");
            }


            Account account = new Account
            {
                Balance = _accountOptions.InitialBalance,
                Currency = request.Currency,
                IbanCode = request.IbanCode
            };

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
            account.PersonId = person.PersonId;


            _dbContext.Accounts.Add(account);

            _dbContext.SaveChanges();

            AccountCreated accountCreated = new(request.IbanCode, request.AccountType);
            await _mediator.Publish(accountCreated,cancellationToken);

            return Unit.Value;
        }

        
    }
}
