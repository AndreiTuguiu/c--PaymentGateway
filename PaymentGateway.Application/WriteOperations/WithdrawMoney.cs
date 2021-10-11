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
    public class WithdrawMoney : IWriteOperation<WithdrawMoneyCommand>
    {
        public IEventSender eventSender;
        public WithdrawMoney(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(WithdrawMoneyCommand operation)
        {
            Database database = Database.GetInstance();
            Account account;
            if (operation.AccountId.HasValue)
            {
                account = database.Accounts.FirstOrDefault(x => x.AccountId == operation.AccountId);
            }
            else
            {
                account = database.Accounts.FirstOrDefault(x => x.IbanCode == operation.IbanConde);
            }
            if (account == null)
            {
                throw new Exception("Account not found!");
            }

            if (operation.Amount <= 0)
            {
                throw new Exception("Cannot withdraw amount");
            }

            if (!String.IsNullOrEmpty(operation.Currency))
            {
                account = database.Accounts.FirstOrDefault(x => x.Currency == operation.Currency);
            }

            if(account.Balance< operation.Amount)
            {
                throw new Exception("Invalid withdrawal amount!");
            }

            Transaction transaction = new Transaction();
            transaction.TransactionId = database.Transactions.Count() + 1;
            transaction.Currency = operation.Currency;
            transaction.Amount = operation.Amount;
            transaction.Date = DateTime.Now;
            transaction.Type = TransactionType.Withdraw;

            database.Transactions.Add(transaction);

            account.Balance = account.Balance - transaction.Amount;

            database.SaveChanges();

            WithDrawnMoney withDrawnMoney = new(account.IbanCode, account.Balance, account.Currency);
            eventSender.SendEvent(withDrawnMoney);
        }
    }
}
