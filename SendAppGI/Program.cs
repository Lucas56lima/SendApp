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

            // Configurar a compatibilidade de renderiza��o antes da cria��o de qualquer janela
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
                    services.AddSingleton<DataStoreService>(); // Servi�o para o banco
                    services.AddTransient<InitialViewModel>(); // ViewModel
                    services.AddTransient<Initial>();          // Formul�rio principal

                    // Registrar o IMemoryCache
                    services.AddMemoryCache();

                    // Registrar o HttpClient
                    services.AddHttpClient(); // Agora deve funcionar ap�s a instala��o do pacote
                });
    }
}
