using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class DepositMoney : IRequestHandler<DepositMoneyCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly Database _database;
        public DepositMoney(IEventSender eventSender,Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }

        public Task<Unit> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            Account account;
            if (request.AccountId.HasValue)
            {
                account = _database.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = _database.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanConde);
            }
            if (account == null)
            {
                throw new Exception("Account not found!");
            }

            if (request.Amount <= 0)
            {
                throw new Exception("Cannot deposit negative amount");
            }

            if (!String.IsNullOrEmpty(request.Currency))
            {
                account = _database.Accounts.FirstOrDefault(x => x.Currency == request.Currency);
            }


            Transaction transaction = new Transaction();
            transaction.TransactionId = _database.Transactions.Count() + 1;
            transaction.Currency = account.Currency;
            transaction.Amount = request.Amount;
            transaction.Type = TransactionType.Deposit;
            transaction.Date = DateTime.Now;


            _database.Transactions.Add(transaction);

            account.Balance = account.Balance + transaction.Amount;

            _database.SaveChanges();

            DepositedMoney depMoney = new(request.IbanConde, account.Balance, request.Currency);
            _eventSender.SendEvent(depMoney);

            return Unit.Task;
        }

        
    }
}
