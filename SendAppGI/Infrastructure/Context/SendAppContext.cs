using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class SendAppContext(DbContextOptions<SendAppContext> options) : DbContext(options)
    {        
        public DbSet<Store> Store { get; set; }
        public DbSet<Scheduling> Schedulings { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
