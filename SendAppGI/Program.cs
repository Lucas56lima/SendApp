using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;
using SendAppGI.Services;
using SendAppGI.Viewsmodels;

namespace SendAppGI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Configurar o Host para DI
            var host = CreateHostBuilder().Build();

            // Configurar a compatibilidade de renderização antes da criação de qualquer janela
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
                    services.AddSingleton<DataStoreService>(); // Serviço para o banco
                    services.AddTransient<InitialViewModel>(); // ViewModel
                    services.AddTransient<Initial>();          // Formulário principal

                    // Registrar o IMemoryCache
                    services.AddMemoryCache();

                    // Registrar o HttpClient
                    services.AddHttpClient(); // Agora deve funcionar após a instalação do pacote
                });
    }
}
