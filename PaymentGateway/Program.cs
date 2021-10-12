using Abstractions;
using ExternalService;
using PaymentGateway.Application;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;

namespace PaymentGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            Account firstAccount = new Account();
            firstAccount.Balance = 100;

            Console.WriteLine(firstAccount.Balance);
            IEventSender eventSender = new EventSender();
            AccountOptions accOpt = new AccountOptions();

            EnrollCustomerOperation client = new EnrollCustomerOperation(eventSender);
            EnrollCustomerCommand command = new EnrollCustomerCommand();

            command.Name = "Popescu";
            command.UniqueIdentifier = "1234567898765";
            command.ClientType="Individual";
           // command.AccountType = "Activ";
            command.Currency = "RON";

            

            Console.WriteLine(command.Name);
            Console.WriteLine(command.UniqueIdentifier);
            Console.WriteLine(command.ClientType);
            Console.WriteLine(command.AccountType);
            Console.WriteLine(command.Currency);

            client.PerformOperation(command);


            CreateAccount createAccount = new CreateAccount(eventSender,accOpt);
            CreateAccountCommand createAccountCommand = new CreateAccountCommand();

            createAccountCommand.PersonId = 0;
            createAccountCommand.Currency = "RON";
            createAccountCommand.IbanCode = "RO12345678987";
            createAccountCommand.AccountType = "Investment";

            Console.WriteLine(createAccountCommand.PersonId);
            Console.WriteLine(createAccountCommand.IbanCode);
            Console.WriteLine(createAccountCommand.AccountType);
           

            createAccount.PerformOperation(createAccountCommand);


            DepositMoney depMon = new DepositMoney(eventSender);
            DepositMoneyCommand depMonComm = new DepositMoneyCommand();
            depMonComm.AccountId = 0;
            depMonComm.IbanConde = "RO12345678987";
            depMonComm.Currency = "RON";
            depMonComm.Amount = 10000;

            depMon.PerformOperation(depMonComm);


            WithdrawMoney withdrawMoney = new WithdrawMoney(eventSender);
            WithdrawMoneyCommand withdraw = new WithdrawMoneyCommand();
            withdraw.IbanConde = "RO12345678987";
            withdraw.Currency = "RON";
            withdraw.Amount = 3000;
            withdraw.AccountId = 0;

            withdrawMoney.PerformOperation(withdraw);

            CreateProduct product = new CreateProduct(eventSender);
            CreateProductCommand createProduct = new CreateProductCommand();
            createProduct.Name = "Adidasi";
            createProduct.ProductId = 0;
            createProduct.Value = 160;
            createProduct.Currency = "RON";
            createProduct.Limit = 4;

            product.PerformOperation(createProduct);

            CreateProduct product1 = new CreateProduct(eventSender);
            CreateProductCommand createProduct1 = new CreateProductCommand();
            createProduct1.Name = "Blugi";
            createProduct.ProductId = 1;
            createProduct.Value = 100;
            createProduct.Currency = "RON";
            createProduct.Limit = 3;

            product1.PerformOperation(createProduct1);

            PurchaseProduct purchase = new PurchaseProduct(eventSender);
            PurchaseProductCommand purchaseProductCommand = new PurchaseProductCommand();
            purchaseProductCommand.AccountId = 0;
            purchaseProductCommand.ProductId = 0;
            purchaseProductCommand.Quantity = 3;

            purchase.PerformOperation(purchaseProductCommand);


            Console.ReadLine();


            
        }
    }
}
