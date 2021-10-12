using Abstractions;
using ExternalService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.IO;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .Build();

            // setup
            var services = new ServiceCollection();
            services.RegisterBusinessServices(Configuration);

            services.AddSingleton<IEventSender, EventSender>();
            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();

            // use
            var command = new EnrollCustomerCommand
            {
                ClientType = "Individual",
                AccountType = "Current",
                Name = "YEP",
                Currency = "RON",
                UniqueIdentifier = "23",
            };

            var client = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
           
            client.PerformOperation(command);


            var createAccountCommand = new CreateAccountCommand
            {
                IbanCode="RO123456789",
                PersonId= 0,
                AccountType = "Investment",
                Currency = "RON"
                
            };
            var createAccount = serviceProvider.GetRequiredService<CreateAccount>();


            createAccount.PerformOperation(createAccountCommand);


            var depMonComm= new DepositMoneyCommand
            {
                AccountId=1,
                IbanConde = "RO123456789",
                Amount = 10000,
                Currency="RON"
            };
            var depMon = serviceProvider.GetRequiredService<DepositMoney>();

            depMon.PerformOperation(depMonComm);


            var withdraw = new WithdrawMoneyCommand
            {
                AccountId = 1,
                IbanConde = "RO123456789",
                Amount = 3000,
                Currency = "RON"
            };

            var withdrawMoney = serviceProvider.GetRequiredService<WithdrawMoney>();

            withdrawMoney.PerformOperation(withdraw);


            var createProduct = new CreateProductCommand
            {
                Name = "Adidasi",
                ProductId = 0,
                Value = 160,
                Currency = "RON",
                Limit = 4
            };

            var product = serviceProvider.GetRequiredService<CreateProduct>();

            product.PerformOperation(createProduct);


            var createProduct1 = new CreateProductCommand
            {
                Name = "Blugi",
                ProductId = 1,
                Value = 100,
                Currency = "RON",
                Limit = 3
            };

            var product1 = serviceProvider.GetRequiredService<CreateProduct>();

            product1.PerformOperation(createProduct1);


            var purchaseProductCommand = new PurchaseProductCommand
            {
                AccountId = 0,
                ProductId = 0,
                Quantity = 3
            };

            var purchase = serviceProvider.GetRequiredService<PurchaseProduct>();

            purchase.PerformOperation(purchaseProductCommand);


            Console.ReadLine();


            
        }
    }
}
