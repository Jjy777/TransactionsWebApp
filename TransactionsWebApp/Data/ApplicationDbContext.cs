using Microsoft.EntityFrameworkCore;
using TransactionsWebApp.Model;

namespace TransactionsWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 
        { 
        } 
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<HistoryLog> HistoryLog { get; set; }

    }
}
 