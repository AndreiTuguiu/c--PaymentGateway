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
    public class PurchaseProduct : IRequestHandler<PurchaseProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;
        public PurchaseProduct(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {
            var account = _dbContext.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);

            if (account == null)
            {
                throw new Exception("Account not found");
            }
            double totalValue;
            string currency;
            var product = _dbContext.Products.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            if (product.Limit < request.Quantity)
            {
                throw new Exception("Not enough in stock");
            }
            totalValue = product.Value * request.Quantity;
            currency = product.Currency;
            product.Limit -= request.Quantity;

            if (totalValue > account.Balance)
            {
                throw new Exception("Not enough Balance");
            }

            Transaction transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = totalValue,
                Currency = currency,
                Date = DateTime.Now,
                Type = TransactionType.PurchaseService,
            };

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            account.Balance -= totalValue;

            ProductXtransaction pxt = new ProductXtransaction
            {
                TransactionId = transaction.TransactionId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Currency= product.Currency
            };

            _dbContext.ProductXtransactions.Add(pxt);

            _dbContext.SaveChanges();

            ProductPurchased productPurchased = new(account.AccountId, pxt.ProductId, pxt.Quantity, totalValue);
            await _mediator.Publish(productPurchased,cancellationToken);
            return Unit.Value;
        }

    }
}
