using CateringManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CateringManagement.Data
{
    public class CateringContext : DbContext
    {
        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        //public CateringContext(DbContextOptions<CateringContext> options, IHttpContextAccessor httpContextAccessor)
        //    : base(options)
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //    UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
        //    UserName = UserName ?? "Unknown";
        //}

        public CateringContext(DbContextOptions<CateringContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
                UserName ??= "Unknown";
            }
            else
            {
                //No HttpContext so seeding data
                UserName = "Seed Data";
            }
        }

        public CateringContext(DbContextOptions<CateringContext> options) 
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<FunctionType> FunctionTypes { get; set; }
        public DbSet<Function> Functions { get; set; }

        public DbSet<FunctionRoom> FunctionRooms { get; set; } //added DbSets
        public DbSet<Room> Rooms { get; set; }
        public DbSet<MealType> MealTypes { get; set; }

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


            //Prevent Cascade Delete from MealType to Function
            //so we are prevented from deleting a MealType with
            //Functions assigned
            modelBuilder.Entity<MealType>()
                .HasMany<Function>(ft => ft.Functions)
                .WithOne(f => f.MealType)
                .HasForeignKey(f => f.MealTypeID)
                .OnDelete(DeleteBehavior.Restrict);


            //Many to Many Intersection
            modelBuilder.Entity<FunctionRoom>()
            .HasKey(t => new { t.FunctionID, t.RoomID });

            //Prevent Cascade Delete from Room to FunctionRoom
            //so we are prevented from deleting a Room with
            //Functions assigned
            modelBuilder.Entity<Room>() 
                .HasMany<FunctionRoom>(c => c.FunctionRooms)
                .WithOne(f => f.Room)
                .HasForeignKey(f => f.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }
}
