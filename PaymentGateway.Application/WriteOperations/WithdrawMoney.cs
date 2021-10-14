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
    public class WithdrawMoney : IRequestHandler<WithdrawMoneyCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly Database _database;
        public WithdrawMoney(IEventSender eventSender,Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }

        public Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
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
                throw new Exception("Cannot withdraw amount");
            }

            if (!String.IsNullOrEmpty(request.Currency))
            {
                account = _database.Accounts.FirstOrDefault(x => x.Currency == request.Currency);
            }

            if (account.Balance < request.Amount)
            {
                throw new Exception("Invalid withdrawal amount!");
            }

            Transaction transaction = new Transaction();
            transaction.TransactionId = _database.Transactions.Count() + 1;
            transaction.Currency = request.Currency;
            transaction.Amount = request.Amount;
            transaction.Date = DateTime.Now;
            transaction.Type = TransactionType.Withdraw;

            _database.Transactions.Add(transaction);

            account.Balance = account.Balance - transaction.Amount;

            _database.SaveChanges();

            WithDrawnMoney withDrawnMoney = new(account.IbanCode, account.Balance, account.Currency);
            _eventSender.SendEvent(withDrawnMoney);

            return Unit.Task;
        }

    }
}
