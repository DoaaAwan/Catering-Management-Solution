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

            //Prevent Cascade Delete from Room to FunctionRoom
            //so we are prevented from deleting a Room with
            //Functions assigned
            modelBuilder.Entity<Room>() 
                .HasMany<FunctionRoom>(c => c.FunctionRooms)
                .WithOne(f => f.Room)
                .HasForeignKey(f => f.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from MealType to Function
            //so we are prevented from deleting a MealType with
            //Functions assigned
            modelBuilder.Entity<MealType>()
                .HasMany<Function>(ft => ft.Functions)
                .WithOne(f => f.MealType)
                .HasForeignKey(f => f.MealTypeID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
