using Microsoft.EntityFrameworkCore;
using RMSWebApi.Models;

namespace RMSWebApi.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        // One line per table (names must match the SQL tables)
        public DbSet<UsersR> UsersR => Set<UsersR>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Staff> Staffs => Set<Staff>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    }
}
