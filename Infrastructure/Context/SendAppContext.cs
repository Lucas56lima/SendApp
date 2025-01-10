using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class SendAppContext : DbContext
    {
        public SendAppContext(DbContextOptions<SendAppContext> options) : base(options) { }
        
        public DbSet<Store> Store { get; set; }
        public DbSet<Scheduling> Schedulings { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
