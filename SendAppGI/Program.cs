using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendAppGI.Services;
using SendAppGI.Viewmodels;

namespace SendAppGI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Configurar o Host para DI
            var host = CreateHostBuilder().Build();

            Application.SetCompatibleTextRenderingDefault(false);

            // Inicializar a configura��o do aplicativo
            ApplicationConfiguration.Initialize();

            // Resolver a inst�ncia do formul�rio principal
            
            var services = host.Services;
            var mainForm = services.GetRequiredService<Initial>();

            // Rodar a aplica��o            
            Application.Run(mainForm);
           
        }

        static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    // Registrar os servi�os necess�rios
                    services.AddSingleton<DataStoreService>();
                    services.AddSingleton<MailService>();
                    services.AddSingleton<FileService>();
                    services.AddTransient<InitialViewModel>(); // ViewModel
                    services.AddTransient<Initial>();          // Formul�rio principal
                    
                    // Registrar o IMemoryCache
                    services.AddMemoryCache();

                    // Registrar o HttpClient
                    services.AddHttpClient(); // Agora deve funcionar ap�s a instala��o do pacote
                });
    }
}
