using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace PaymentGateway.Application.CommandHandlers
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;
        public EnrollCustomerOperation(IMediator mediator,PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {
            var person = new Person
            {
                Cnp = request.UniqueIdentifier,
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
            

            _dbContext.Persons.Add(person);
            _dbContext.SaveChanges();

            Account account = new Account
            {
                PersonId = person.PersonId,
                Type = AccountType.Current,
                Currency = request.Currency,
                Balance = 0,
                IbanCode = request.IbanCode
            };

            _dbContext.Accounts.Add(account);

            _dbContext.SaveChanges();
            CustomerEnrolled eventCustEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType);
            await _mediator.Publish(eventCustEnroll, cancellationToken);

            return Unit.Value;
        }

    }
}
