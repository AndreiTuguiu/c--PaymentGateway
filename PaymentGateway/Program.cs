using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.IO;
using MediatR;

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

            //services.AddSingleton<IEventSender, AllEventHandler>();
            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();
            var m = serviceProvider.GetRequiredService<IMediator>();
            // use
            var command = new EnrollCustomerCommand
            {
                ClientType = "Individual",
                AccountType = "Current",
                Name = "YEP",
                Currency = "RON",
                UniqueIdentifier = "23",
                IbanCode= "RO987654321"
            };

            
           
            m.Send(command, default).GetAwaiter().GetResult();


            var createAccountCommand = new CreateAccountCommand
            {
                IbanCode="RO123456789",
                PersonId= 1,
                AccountType = "Investment",
                Currency = "RON"
                
            };
            


            m.Send(createAccountCommand, default).GetAwaiter().GetResult();


            var depMonComm= new DepositMoneyCommand
            {
                AccountId=2,
                IbanConde = "RO123456789",
                Amount = 10000,
                Currency="RON"
            };
            

            m.Send(depMonComm, default).GetAwaiter().GetResult();


            var withdraw = new WithdrawMoneyCommand
            {
                AccountId = 2,
                IbanConde = "RO123456789",
                Amount = 3000,
                Currency = "RON"
            };

           

            m.Send(withdraw, default).GetAwaiter().GetResult();


            var createProduct = new CreateProductCommand
            {
                Name = "Adidasi",
                ProductId = 0,
                Value = 160,
                Currency = "RON",
                Limit = 4
            };

            

            m.Send(createProduct, default).GetAwaiter().GetResult();


            var createProduct1 = new CreateProductCommand
            {
                Name = "Blugi",
                ProductId = 1,
                Value = 100,
                Currency = "RON",
                Limit = 3
            };

            

            m.Send(createProduct1, default).GetAwaiter().GetResult();


            var purchaseProductCommand = new PurchaseProductCommand
            {
                AccountId = 0,
                ProductId = 0,
                Quantity = 3
            };

            

            m.Send(purchaseProductCommand, default).GetAwaiter().GetResult();


            var query = new Application.Queries.ListOfAccounts.Query
            {
                PersonId = 1
            };

            
            var result = m.Send(query, default).GetAwaiter().GetResult();


            Console.ReadLine();


            
        }
    }
}
