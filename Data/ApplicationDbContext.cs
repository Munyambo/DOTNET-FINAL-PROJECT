using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;

namespace Restaurant_Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) { }

        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Staff> Staffs => Set<Staff>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // These 7 tables were created with SQL, so the migration must skip them
            builder.Entity<Branch>().ToTable("Branches", t => t.ExcludeFromMigrations());
            builder.Entity<Category>().ToTable("Categories", t => t.ExcludeFromMigrations());
            builder.Entity<Customer>().ToTable("Customers", t => t.ExcludeFromMigrations());
            builder.Entity<Staff>().ToTable("Staffs", t => t.ExcludeFromMigrations());
            builder.Entity<MenuItem>().ToTable("MenuItems", t => t.ExcludeFromMigrations());
            builder.Entity<Order>().ToTable("Orders", t => t.ExcludeFromMigrations());
            builder.Entity<OrderItem>().ToTable("OrderItems", t => t.ExcludeFromMigrations());
        }

    }
}
