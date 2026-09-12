using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // Passa a herdar de IdentityDbContext<User> em vez de DbContext: já traz
    // todas as DbSets das tabelas de Identity (AspNetUsers, AspNetRoles,
    // AspNetUserRoles, etc.), não é preciso declará-las manualmente.
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
