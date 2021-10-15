using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.IO;
using MediatR;
using PaymentGateway.Application.Queries;
using ExternalService;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using PaymentGateway.PublishedLanguage.Event;
using FluentValidation;
using MediatR.Pipeline;
using PaymentGateway.WebApi.MediatorPipeline;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .Build();

            // setup
            var services = new ServiceCollection();

            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;

            services.RegisterBusinessServices(Configuration);

            services.Scan(scan => scan
            .FromAssemblyOf<ListOfAccounts>()
            .AddClasses(classes => classes.AssignableTo<IValidator>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());


            
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));


            services.AddScopedContravariant<INotificationHandler<INotification>, AllEventHandler>(typeof(CustomerEnrolled).Assembly);

            services.AddMediatR(typeof(ListOfAccounts).Assembly, typeof(AllEventHandler).Assembly);

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

            
           
            await m.Send(command, cancellationToken);


            var createAccountCommand = new CreateAccountCommand
            {
                IbanCode="RO123456789",
                PersonId= 1,
                AccountType = "Investment",
                Currency = "RON"
                
            };
            


            await m.Send(createAccountCommand, cancellationToken);


            var depMonComm= new DepositMoneyCommand
            {
                AccountId=2,
                IbanConde = "RO123456789",
                Amount = 10000,
                Currency="RON"
            };
            

            await m.Send(depMonComm, cancellationToken);


            var withdraw = new WithdrawMoneyCommand
            {
                AccountId = 2,
                IbanConde = "RO123456789",
                Amount = 3000,
                Currency = "RON"
            };

           

            await m.Send(withdraw, cancellationToken);


            var createProduct = new CreateProductCommand
            {
                Name = "Adidasi",
                ProductId = 0,
                Value = 160,
                Currency = "RON",
                Limit = 4
            };

            

            await m.Send(createProduct, cancellationToken);


            var createProduct1 = new CreateProductCommand
            {
                Name = "Blugi",
                ProductId = 1,
                Value = 100,
                Currency = "RON",
                Limit = 3
            };

            

            await m.Send(createProduct1, cancellationToken);


            var purchaseProductCommand = new PurchaseProductCommand
            {
                AccountId = 0,
                ProductId = 0,
                Quantity = 3
            };

            

            m.Send(purchaseProductCommand, default).GetAwaiter().GetResult();


            var query = new ListOfAccounts.Query
            {
                PersonId = 1
            };

            
            var result =await m.Send(query, cancellationToken);


            Console.ReadLine();


            
        }
    }
}
