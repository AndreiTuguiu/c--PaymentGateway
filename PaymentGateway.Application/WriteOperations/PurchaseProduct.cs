using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class PurchaseProduct : IWriteOperation<PurchaseProductCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly Database _database;
        public PurchaseProduct(IEventSender eventSender, Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }
        public void PerformOperation(PurchaseProductCommand operation)
        {
           
            var account = _database.Accounts.FirstOrDefault(x => x.AccountId == operation.AccountId);

            if(account== null)
            {
                throw new Exception("Account not found");
            }
            double totalValue;
            string currency;
            var product = _database.Products.FirstOrDefault(x => x.ProductId == operation.ProductId);
            if(product== null)
            {
                throw new Exception("Product not found");
            }
            if(product.Limit< operation.Quantity)
            {
                throw new Exception("Not enough in stock");
            }
            totalValue = product.Value*operation.Quantity;
            currency = product.Currency;
            product.Limit -= operation.Quantity;

            if(totalValue> account.Balance)
            {
                throw new Exception("Not enough Balance");
            }

            Transaction transaction = new Transaction();
            transaction.Amount = totalValue;
            transaction.Currency = currency;
            transaction.Date = DateTime.Now;
            transaction.Type = TransactionType.PurchaseService;
            transaction.TransactionId = _database.Transactions.Count() + 1;

            _database.Transactions.Add(transaction);

            account.Balance -= totalValue;

            ProductXTransaction pxt = new ProductXTransaction();
            pxt.TransactionId = transaction.TransactionId;
            pxt.ProductId = operation.ProductId;
            pxt.Quantity = operation.Quantity;

            _database.ProductXTransactions.Add(pxt);

            _database.SaveChanges();

            ProductPurchased productPurchased = new(account.AccountId, pxt.ProductId, pxt.Quantity, totalValue);
            _eventSender.SendEvent(productPurchased);
            
        }
    }
}
