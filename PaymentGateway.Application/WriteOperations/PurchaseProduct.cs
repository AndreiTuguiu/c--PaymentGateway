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
    public class PurchaseProduct : IRequestHandler<PurchaseProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public PurchaseProduct(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {
            var account = _database.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);

            if (account == null)
            {
                throw new Exception("Account not found");
            }
            double totalValue;
            string currency;
            var product = _database.Products.FirstOrDefault(x => x.ProductId == request.ProductId);
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
                Amount = totalValue,
                Currency = currency,
                Date = DateTime.Now,
                Type = TransactionType.PurchaseService,
                TransactionId = _database.Transactions.Count() + 1
            };

            _database.Transactions.Add(transaction);

            account.Balance -= totalValue;

            ProductXTransaction pxt = new ProductXTransaction
            {
                TransactionId = transaction.TransactionId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            _database.ProductXTransactions.Add(pxt);

            _database.SaveChanges();

            ProductPurchased productPurchased = new(account.AccountId, pxt.ProductId, pxt.Quantity, totalValue);
            await _mediator.Publish(productPurchased,cancellationToken);
            return Unit.Value;
        }

    }
}
