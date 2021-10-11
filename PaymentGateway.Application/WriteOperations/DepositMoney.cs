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
    public class DepositMoney : IWriteOperation<DepositMoneyCommand>
    {
        public IEventSender eventSender;

        public DepositMoney(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(DepositMoneyCommand operation)
        {
            Database db = Database.GetInstance();
            Account account;
            if (operation.AccountId.HasValue)
            {
                account = db.Accounts.FirstOrDefault(x => x.AccountId == operation.AccountId);
            }
            else
            {
                account = db.Accounts.FirstOrDefault(x => x.IbanCode == operation.IbanConde);
            }
            if (account == null)
            {
                throw new Exception("Account not found!");
            }

            if (operation.Amount <= 0)
            {
                throw new Exception("Cannot deposit negative amount");
            }

            if (!String.IsNullOrEmpty(operation.Currency))
            {
                account = db.Accounts.FirstOrDefault(x => x.Currency == operation.Currency);
            }


            Transaction transaction = new Transaction();
            transaction.TransactionId = db.Transactions.Count() + 1;
            transaction.Currency = account.Currency;
            transaction.Amount = operation.Amount;
            transaction.Type = TransactionType.Deposit;
            transaction.Date = DateTime.Now;


            db.Transactions.Add(transaction);

            account.Balance = account.Balance + transaction.Amount;

            db.SaveChanges();

            DepositedMoney depMoney = new(operation.IbanConde, account.Balance, operation.Currency);
            eventSender.SendEvent(depMoney);

        }
    }
}
