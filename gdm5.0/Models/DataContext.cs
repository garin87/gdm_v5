using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace gdm5._0.Models
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<ProductParameter> ProductParameters { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceListValue> PriceListValues { get; set; }
        public DbSet<ComplexPriceList> ComplexPriceLists { get; set; }
        public DbSet<Dictionary> Dictionary { get; set; }
        public DbSet<Label> Label { get; set; }
        public DbSet<LabelCategory> LabelCategory { get; set; }
        public DbSet<WareHouse> WareHouse { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Label>()
                .HasOne(c => c.LabelCategory)
                .WithMany(e => e.Labels)
                .HasForeignKey(c => c.LabelCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Label>()
                .HasOne(c => c.Dictionary)
                .WithMany(e => e.Labels)
                .HasForeignKey(c => c.DictionaryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PriceList>()
               .HasOne(c => c.ComplexPriceList)
               .WithMany(e => e.PriceList)
               .HasForeignKey(c => c.ComplexPriceListId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PriceListValue>()
               .HasOne(c => c.PriceList)
               .WithMany(e => e.PriceListValue)
               .HasForeignKey(c => c.PriceListId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PriceListValue>()
               .HasOne(c => c.Product)
               .WithMany(e => e.PriceListValue)
               .HasForeignKey(c => c.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
               .HasOne(c => c.Currency)
               .WithMany(e => e.Products)
               .HasForeignKey(c => c.CurrencyId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parameter>()
               .HasOne(Pr => Pr.ProductType)
               .WithMany(Pt => Pt.Parameters)
               .HasForeignKey(Pr => Pr.ProductTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductParameter>()
               .HasOne(bc => bc.Product)
               .WithMany(b => b.ProductParameters)
               .HasForeignKey("ProductId")
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductParameter>()
               .HasOne(bc => bc.Parameter)
               .WithMany(c => c.ProductParameters)
               .HasForeignKey("ParameterId")
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
               .HasOne(c => c.Customer)
               .WithMany(e => e.Orders)
               .HasForeignKey(c => c.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
               .HasOne(c => c.Currency)
               .WithMany(e => e.Orders)
               .HasForeignKey(c => c.CurrencyId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderProduct>()
               .HasOne(bc => bc.Order)
               .WithMany(b => b.OrderProduct)
               .HasForeignKey("OrderId")
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderProduct>()
               .HasOne(bc => bc.Product)
               .WithMany(c => c.OrderProduct)
               .HasForeignKey("ProductId")
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CurrencyRate>()
               .HasOne(c => c.Currency)
               .WithMany(e => e.CurrencyRates)
               .HasForeignKey(c => c.CurrencyId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()
               .HasOne(c => c.Role)
               .WithMany(e => e.User)
               .HasForeignKey(c => c.RoleId)
               .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);
        }

        internal Product FirstOrDefault(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }

    }
}
