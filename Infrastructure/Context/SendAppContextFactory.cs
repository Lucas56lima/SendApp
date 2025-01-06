using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Context
{
    public class SendAppContextFactory : IDesignTimeDbContextFactory<SendAppContext>
    {
        string conection = "caminho do banco";

        public SendAppContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SendAppContext>();
            optionsBuilder.UseSqlite(conection);
            return new SendAppContext(optionsBuilder.Options);
        }
    }
}
