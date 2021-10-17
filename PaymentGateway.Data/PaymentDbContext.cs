using PaymentGateway.Models;
using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) :base(options)
        {

        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ProductXTransaction> ProductXTransactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasKey(x => x.ProductId);
            modelBuilder.Entity<Product>().Property(x => x.ProductId);//.HasColumnName("IdUlMeuSpecial");

            modelBuilder.Entity<ProductXTransaction>().HasKey(x => new { x.ProductId, x.TransactionId });

            modelBuilder.ApplyConfiguration(new PersonConfiguration());
        }

        /*private static Database _instance;
        public static Database GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Database();
            }

            return _instance;
        }*/

        //public void SaveChanges()
        //{
        //    Console.WriteLine("Save changes to database");
        //}

    }
}
