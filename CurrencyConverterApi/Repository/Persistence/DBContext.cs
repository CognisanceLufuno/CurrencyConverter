using CurrencyConverterApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConverterApi.Repository.Persistence
{
    public class DBContext(DbContextOptions<DBContext> context) : DbContext(context)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<ConversionsHistory> ConversionsHistory { get; set; }
    }
}
