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
    public class WithdrawMoney : IRequestHandler<WithdrawMoneyCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;
        public WithdrawMoney(IMediator mediator,PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
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
                throw new Exception("Cannot withdraw amount");
            }

            if (!String.IsNullOrEmpty(request.Currency))
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.Currency == request.Currency);
            }

            if (account.Balance < request.Amount)
            {
                throw new Exception("Invalid withdrawal amount!");
            }

            Transaction transaction = new Transaction
            {
                AccountId=account.AccountId,
                Currency = request.Currency,
                Amount = request.Amount,
                Date = DateTime.Now,
                Type = TransactionType.Withdraw
            };

            _dbContext.Transactions.Add(transaction);

            account.Balance = account.Balance - transaction.Amount;

            _dbContext.SaveChanges();

            WithDrawnMoney withDrawnMoney = new(account.IbanCode, account.Balance, account.Currency);
            await _mediator.Publish(withDrawnMoney,cancellationToken);

            return Unit.Value;
        }

    }
}
