using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SQLitePCL;

namespace Infrastructure.Context
{
    public class SendAppContextFactory : IDesignTimeDbContextFactory<SendAppContext>
    {
        string conection = "Data Source=C:\\Users\\Usuário\\source\\repos\\SendApp\\Infrastructure\\Database\\Database.db";

        public SendAppContext CreateDbContext(string[] args)
        {
            Batteries.Init();
            var optionsBuilder = new DbContextOptionsBuilder<SendAppContext>();
            optionsBuilder.UseSqlite(conection);
            return new SendAppContext(optionsBuilder.Options);
        }
    }
}
