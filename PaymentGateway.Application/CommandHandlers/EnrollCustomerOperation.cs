using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.CommandHandlers
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public EnrollCustomerOperation(IMediator mediator,Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {
            var person = new Person
            {
                CNP = request.UniqueIdentifier,
                Name = request.Name
            };
            if (request.ClientType == "Company")
            {
                person.Type = PersonType.Company;
            }
            else if (request.ClientType == "Individual")
            {
                person.Type = PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupport person type!");
            }

            person.PersonId = _database.Persons.Count + 1;

            _database.Persons.Add(person);

            Account account = new Account
            {
                Type = AccountType.Current,
                Currency = request.Currency,
                Balance = 0,
                IbanCode = request.IbanCode
            };

            _database.Accounts.Add(account);

            _database.SaveChanges();
            CustomerEnrolled eventCustEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType);
            await _mediator.Publish(eventCustEnroll, cancellationToken);

            return Unit.Value;
        }

    }
}
