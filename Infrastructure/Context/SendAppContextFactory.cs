using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SQLitePCL;
using System.Reflection;

namespace Infrastructure.Context
{
    public class SendAppContextFactory : IDesignTimeDbContextFactory<SendAppContext>
    {
        public SendAppContext CreateDbContext(string[] args)
        {            
            Batteries.Init();
            
            string appInstallationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
           
            string databaseFilePath = Path.Combine(appInstallationPath, "Infrastructure", "Database", "Database.db");
           
            if (!File.Exists(databaseFilePath))
            {
                throw new FileNotFoundException("O banco de dados não foi encontrado no caminho especificado.", databaseFilePath);
            }
            string connectionString = $"Data Source={databaseFilePath}";           
            var optionsBuilder = new DbContextOptionsBuilder<SendAppContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new SendAppContext(optionsBuilder.Options);
        }
    }
}
