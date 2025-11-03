using BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp
{
    public class BlazorAppContext : DbContext
    {
        public BlazorAppContext(DbContextOptions<BlazorAppContext> options) : base(options)
        {

        }

        public DbSet<Customer> Customers => Set<Customer>();
    }
}
