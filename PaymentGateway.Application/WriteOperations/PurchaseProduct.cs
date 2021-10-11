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
        public IEventSender eventSender;
        public PurchaseProduct(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(PurchaseProductCommand operation)
        {
            Database database = Database.GetInstance();
            var account = database.Accounts.FirstOrDefault(x => x.AccountId == operation.AccountId);

            if(account== null)
            {
                throw new Exception("Account not found");
            }
            double totalValue;
            string currency;
            var product = database.Products.FirstOrDefault(x => x.ProductId == operation.ProductId);
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
            transaction.TransactionId = database.Transactions.Count() + 1;

            database.Transactions.Add(transaction);

            account.Balance -= totalValue;

            ProductXTransaction pxt = new ProductXTransaction();
            pxt.TransactionId = transaction.TransactionId;
            pxt.ProductId = operation.ProductId;
            pxt.Quantity = operation.Quantity;

            database.ProductXTransactions.Add(pxt);

            database.SaveChanges();

            ProductPurchased productPurchased = new(account.AccountId, pxt.ProductId, pxt.Quantity, totalValue);
            eventSender.SendEvent(productPurchased);
            
        }
    }
}
