using Hotel_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Data
{
    public class HMDbContext : DbContext
    {
        public HMDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<BillingService> BillingServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite primary key for BillingService
            modelBuilder.Entity<BillingService>()
                .HasKey(bs => new { bs.BillingId, bs.ServiceId });

            // Define relationships
            modelBuilder.Entity<BillingService>()
                .HasOne(bs => bs.Billing)
                .WithMany(b => b.BillingServices)
                .HasForeignKey(bs => bs.BillingId);

            modelBuilder.Entity<BillingService>()
                .HasOne(bs => bs.Service)
                .WithMany()
                .HasForeignKey(bs => bs.ServiceId);
        }
    }
}