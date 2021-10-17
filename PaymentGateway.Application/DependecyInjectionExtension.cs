using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Data;

namespace PaymentGateway.Application
{
    public static class DependecyInjectionExtension
    {
        public static void AddPaymentDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            #region db selector code
            /*
             * var connectionString = ...
                var dbProvider = configuration.GetValue<string>("Database:Provider");
                switch (dbProvider)
                {
                    case "PostgreSql":
                        services.AddEntityFrameworkNpgsql();
                        options.UseNpgsql(connectionString, builder => { builder.EnableRetryOnFailure(3); });
                        break;
                    case "Sqlite":
                        options.UseSqlite(connectionString, builder => { });
                        break;
                    case "SqlServer":
                        services.AddEntityFrameworkSqlServer();
                        options.UseSqlServer(connectionString, builder => { builder.EnableRetryOnFailure(3); });
                        break;
                    case "MySql":
                        services.AddEntityFrameworkMySql();
                        options.UseMySql(connectionString, builder => { builder.EnableRetryOnFailure(3); });
                        break;
                    case "Memory":
                        services.AddEntityFrameworkInMemoryDatabase();
                        options.UseInMemoryDatabase(connectionString, builder => { });
                        break;
                    default:
                        throw new System.Exception("Unsupported database provider type. Supported values in appsetting.json / Database / Provider:  PostgreSql, SqlServer, MySql, Sqlite");
                }
            */
            #endregion

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // services.AddDbContext<PaymentDbContext>(options => // worse performance
            services.AddDbContextPool<PaymentDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<EnrollCustomerOperation>();
            //services.AddTransient<CreateAccount>();
            //services.AddTransient<DepositMoney>();
            //services.AddTransient<WithdrawMoney>();
            //services.AddTransient<PurchaseProduct>();
            services.AddSingleton<Data.PaymentDbContext>();

            //services.AddTransient<IValidator<Query>, Validator>();
            

            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var options = new AccountOptions
                {
                    InitialBalance = config.GetValue("AccountOptions:InitialBalance", 0)
                };
                return options;
            });


            return services;
        }
    }
}
