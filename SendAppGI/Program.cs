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

            // Inicializar a configuração do aplicativo
            ApplicationConfiguration.Initialize();

            // Resolver a instância do formulário principal
            
            var services = host.Services;
            var mainForm = services.GetRequiredService<Initial>();

            // Rodar a aplicação            
            Application.Run(mainForm);
           
        }

        static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    // Registrar os serviços necessários
                    services.AddSingleton<DataStoreService>();
                    services.AddSingleton<MailService>();
                    services.AddSingleton<FileService>();
                    services.AddTransient<InitialViewModel>(); // ViewModel
                    services.AddTransient<Initial>();          // Formulário principal
                    
                    // Registrar o IMemoryCache
                    services.AddMemoryCache();

                    // Registrar o HttpClient
                    services.AddHttpClient(); // Agora deve funcionar após a instalação do pacote
                });
    }
}
