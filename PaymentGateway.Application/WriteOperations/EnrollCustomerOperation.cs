﻿using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly Database _database;
        public EnrollCustomerOperation(IEventSender eventSender,Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }

        public Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
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

            Account account = new Account();
            account.Type = AccountType.Current;
            account.Currency = request.Currency;
            account.Balance = 0;
            account.IbanCode = request.IbanCode;

            _database.Accounts.Add(account);

            _database.SaveChanges();
            CustomerEnrolled eventCustEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType);
            _eventSender.SendEvent(eventCustEnroll);

            return Unit.Task;
        }

    }
}
