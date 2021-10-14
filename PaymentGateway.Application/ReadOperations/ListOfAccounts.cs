using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.ReadOperations
{
    public class ListOfAccounts
    {
        public class Validator : IValidator<Query>
        {
            private readonly Database _database;

            public Validator(Database database)
            {
                _database = database;
            }

            public bool Validate(Query input)
            {
                var person = input.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.PersonId == input.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.CNP == input.Cnp);

                return person != null;
            }
        }
        public class Query:IRequest<List<Model>>
        {
            public int? PersonId { get; set; }
            public string Cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly Database _database;
            private readonly IValidator<Query> _validator;

            public QueryHandler(Database database,IValidator<Query> validator)
            {
                _database = database;
                _validator = validator;
            }
            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                var isValid = _validator.Validate(request);
                var person = request.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.CNP == request.Cnp);

                if(person == null)
                {
                    throw new Exception("person not found");
                }

                var db = _database.Accounts.Where(x => x.PersondId == person.PersonId);
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
