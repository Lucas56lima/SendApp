using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;

namespace SendAppGI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Initial());
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStoreRepository, IStoreRepository>();
        }
    }
}