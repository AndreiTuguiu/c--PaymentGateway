using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.CommandHandlers
{
    public class DepositMoney : IRequestHandler<DepositMoneyCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;
        public DepositMoney(IMediator mediator,PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            Account account;
            if (request.AccountId.HasValue)
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanConde);
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
                account = _dbContext.Accounts.FirstOrDefault(x => x.Currency == request.Currency);
            }


            Transaction transaction = new Transaction
            {
                TransactionId = _dbContext.Transactions.Count() + 1,
                Currency = account.Currency,
                Amount = request.Amount,
                Type = TransactionType.Deposit,
                Date = DateTime.Now
            };


            _dbContext.Transactions.Add(transaction);

            account.Balance = account.Balance + transaction.Amount;

            _dbContext.SaveChanges();

            DepositedMoney depMoney = new(request.IbanConde, account.Balance, request.Currency);
            await _mediator.Publish(depMoney, cancellationToken);

            return Unit.Value;
        }

        
    }
}
