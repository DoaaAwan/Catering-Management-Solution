using CateringManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CateringManagement.Data
{
    public class CateringContext : DbContext
    {
        public CateringContext(DbContextOptions<CateringContext> options) 
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<FunctionType> FunctionTypes { get; set; }
        public DbSet<Function> Functions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Prevent Cascade Delete from Customer to Function
            //so we are prevented from deleting a Customer with
            //Functions assigned
            modelBuilder.Entity<Customer>()
                .HasMany<Function>(c => c.Functions)
                .WithOne(f => f.Customer)
                .HasForeignKey(f => f.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from FunctionType to Function
            //so we are prevented from deleting a FunctionType with
            //Functions assigned
            modelBuilder.Entity<FunctionType>()
                .HasMany<Function>(ft => ft.Functions)
                .WithOne(f => f.FunctionType)
                .HasForeignKey(f => f.FunctionTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            //Add a unique index to the CustomerCode
            modelBuilder.Entity<Customer>()
            .HasIndex(c => c.CustomerCode)
            .IsUnique();
        }
    }
}
