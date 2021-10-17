using PaymentGateway.Data;
using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using FluentValidation;

namespace PaymentGateway.Application.Queries
{
    public class ListOfAccounts
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator(PaymentDbContext _dbContext)
            {
                RuleFor(q => q).Must(query =>
                {
                    var person = query.PersonId.HasValue ?
                    _dbContext.Persons.FirstOrDefault(x => x.PersonId == query.PersonId) :
                    _dbContext.Persons.FirstOrDefault(x => x.CNP == query.Cnp);

                    return person != null;
                }).WithMessage("Customer not found");
            }

        }

        public class Validator2: AbstractValidator<Query>
        {
            public Validator2(PaymentDbContext dbContext)
            {
                RuleFor(q => q).Must(person =>
                {
                    return person.PersonId.HasValue || !string.IsNullOrEmpty(person.Cnp) ;
                }).WithMessage("Customer data is invalid ");

                //RuleFor(q => q.Cnp).Must(cnp =>
                //{
                //    return !string.IsNullOrEmpty(cnp);
                //}).WithMessage("CNP is empty");

                RuleFor(q => q.PersonId).Must(personId =>
                {
                    var exists = dbContext.Persons.Any(x => x.PersonId == personId);
                    return exists;
                }).WithMessage("Customer does not exist");
            }
        }
        public class Query:IRequest<List<Model>>
        {
            public int? PersonId { get; set; }
            public string Cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly PaymentDbContext _dbContext;
            

            public QueryHandler(PaymentDbContext dbContext)
            {
                _dbContext = dbContext;
                
            }
            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
               
                var person = request.PersonId.HasValue ?
                    _dbContext.Persons.FirstOrDefault(x => x.PersonId == request.PersonId) :
                    _dbContext.Persons.FirstOrDefault(x => x.CNP == request.Cnp);

                if(person == null)
                {
                    throw new Exception("person not found");
                }

                var db = _dbContext.Accounts.Where(x => x.PersondId == person.PersonId);
                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency=x.Currency,
                    IbanCode=x.IbanCode,
                    AccountId=x.AccountId,
                    Limit=x.Limit,
                    Status=x.Status,
                    Type=x.Type
                }).ToList();

                return Task.FromResult(result);
            }
        }

        public class Model
        {
            public int AccountId { get; set; }
            public int PersondId { get; set; }
            public string IbanCode { get; set; }
            public double Balance { get; set; }
            public string Currency { get; set; }
            public AccountType Type { get; set; }
            public AccountStatus Status { get; set; }
            public double Limit { get; set; }
        }
    }
}
