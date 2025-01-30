using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SendAppGI.Services;
using SendAppGI.Viewmodels;
using Service.Services;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;

namespace SendAppGI
{
    internal static class Program
    {
        private static NotifyIcon notifyIcon;
        private static Initial mainForm;        
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        [STAThread]
        static void Main()
        {
            var host = CreateHostBuilder().Build();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            var services = host.Services;

            Form mainForm;

            if (!IsFirstRunCompleted())
            {
                mainForm = services.GetRequiredService<Register>();
                Application.Run(mainForm);
                CreateFirstRunLockFile();
            }

            InitializeNotifyIcon(services);            
            SetStartup();

            Task.Run(async () =>
            {
                await ProcessSchedulingAsync(services);
                await MonitorFilesAsync(services);
            });

            Task.Run(async () =>
            {
                string repoOwner = "Lucas56lima"; // Coloque o nome de usuário ou organização
                string repoName = "SendApp"; // Coloque o nome do repositório

                string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-agent", "SendApp");
                    var response = await client.GetStringAsync(apiUrl);
                    var release = JObject.Parse(response);

                    string releaseVersion = release["tag_name"].ToString();
                    string downloadUrl = release["assets"][0]["browser_download_url"].ToString();
                    string localVersion = "v1.0.0"; // Substitua pela versão real instalada

                    if (releaseVersion != localVersion)
                    {
                        MessageBox.Show("Nova versão encontrada! Baixando...");
                        BaixarAtualizacao(downloadUrl);
                    }
                    else
                    {
                        Console.WriteLine("Seu aplicativo já está atualizado.");
                    }
                }
            });
            Application.Run();

        }

        static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<DataStoreService>();
                    services.AddSingleton<MailService>();
                    services.AddSingleton<FileService>();
                    services.AddTransient<InitialViewModel>();
                    services.AddTransient<Initial>();
                    services.AddTransient<Register>();
                    services.AddScoped<IStoreRepository, StoreRepository>();
                    services.AddScoped<IStoreService, StoreService>();
                    services.AddScoped<ISchedulingRepository, SchedulingRepository>();
                    services.AddScoped<ISchedulingService, SchedulingService>();
                    services.AddScoped<ILogRepository, LogRepository>();
                    services.AddScoped<ILogService, LogService>();

                    string appInstallationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                    string databaseFilePath = Path.Combine(appInstallationPath, "Database", "Database.db");
                    string connectionDefault = $"Data Source={databaseFilePath}";
                    var optionsBuilder = new DbContextOptionsBuilder<SendAppContext>();
                    optionsBuilder.UseSqlite(connectionDefault);

                    services.AddDbContext<SendAppContext>(options =>
                        options.UseSqlite(connectionDefault));

                    using (var context = new SendAppContext(optionsBuilder.Options))
                    {
                        context.Database.Migrate();
                    }

                    services.AddMemoryCache();
                });

        static async Task ProcessSchedulingAsync(IServiceProvider services)
        {
            try
            {
                var fileService = services.GetRequiredService<FileService>();
                var dataStoreService = services.GetRequiredService<DataStoreService>();
                var mailService = services.GetRequiredService<MailService>();

                // Obtendo dados da loja e do agendamento
                var storeDb = await dataStoreService.GetStoreByIdAsync();
                var schedulingDb = await dataStoreService.GetSchedulingsAsync();
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
               
                if (schedulingDb.TransitionDate == today || schedulingDb.TransitionDate.Month == today.Month)
                {
                    string appDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                    string serviceFilesPath = Path.Combine(appDirectory, "Services", "Files");

                    var filterFiles = Directory.EnumerateFiles(serviceFilesPath, "*.zip", SearchOption.AllDirectories)
                        .OrderByDescending(File.GetLastWriteTime)
                        .ToList();

                    // Enviando e-mails para cada arquivo encontrado
                    if(filterFiles.Count() > 0)
                    {
                        foreach (var filterFile in filterFiles)
                        {
                            // Verifica se o envio de e-mail é bem-sucedido
                            bool emailSent = await mailService.SendMail(storeDb.Email, storeDb.Password, Path.GetFullPath(filterFile), storeDb.Name);
                            if (!emailSent)
                            {
                                MessageBox.Show("Falha ao enviar e-mail.");
                                continue; // Pula para o próximo arquivo
                            }
                            Scheduling scheduling = new()
                            {
                                Store = storeDb.Name
                            };
                            Scheduling schedulingPost = new()
                            {
                                Store = storeDb.Name
                            };
                            bool putScheduling = await dataStoreService.PutSchedulingByIdAsync(schedulingDb.Id, scheduling);
                            bool postScheduling = await dataStoreService.PostSchedulingByIdAsync(schedulingPost);

                        }
                        fileService.DeleteZipFile(Path.GetFullPath(filterFiles.FirstOrDefault()));
                    }                   
                    

                }                

            }
            catch (Exception ex)
            {
                // Captura qualquer exceção que possa ocorrer durante a execução
                MessageBox.Show($"Erro ao processar agendamento: {ex.Message}");                
            }
        }

        static void BaixarAtualizacao(string downloadUrl)
        {
            // Baixe o arquivo de atualização
            using (HttpClient client = new HttpClient())
            {
                var fileData = client.GetByteArrayAsync(downloadUrl).Result;
                System.IO.File.WriteAllBytes("atualizacao.zip", fileData);
                Console.WriteLine("Atualização baixada com sucesso!");

                // Aqui você pode extrair e substituir os arquivos do app
            }
        }

        static async Task MonitorFilesAsync(IServiceProvider services)
        {
            var dataStoreService = services.GetRequiredService<DataStoreService>();
            var fileService = services.GetRequiredService<FileService>();
            var storeDb = await dataStoreService.GetStoreByIdAsync();

            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await fileService.StartWatching(storeDb.Path, storeDb.Name);                    
                    await Task.Delay(1000, cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // Task foi cancelada - tratamento opcional
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no monitoramento de arquivos: {ex.Message}");
            }
        }

        private static void InitializeNotifyIcon(IServiceProvider services)
        {
            if (notifyIcon != null) return;

            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "SendApp"
            };

            var contextMenu = new ContextMenuStrip();
            var abrirMenuItem = new ToolStripMenuItem("Abrir");
            var sairMenuItem = new ToolStripMenuItem("Sair");

            abrirMenuItem.Click += (s, e) => ShowMainWindow(services);
            sairMenuItem.Click += (s, e) => ExitApplication();

            contextMenu.Items.Add(abrirMenuItem);
            contextMenu.Items.Add(sairMenuItem);

            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.DoubleClick += (s, e) => ShowMainWindow(services);
        }

        private static void ExitApplication()
        {
            // Cancelar tarefas ativas antes de sair
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }

            Application.Exit();
        }

        private static void ShowMainWindow(IServiceProvider services)
        {
            if (mainForm == null)
            {
                mainForm = services.GetRequiredService<Initial>();
            }

            if (!mainForm.Visible)
            {
                mainForm.Show();
                mainForm.WindowState = FormWindowState.Normal;
                mainForm.Activate();

                // Cancelar o monitoramento ao abrir o formulário principal
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                }
            }
            else
            {
                mainForm.Activate();
            }
        }

        private static bool IsFirstRunCompleted()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FirstRun.lock");
            return File.Exists(filePath);
        }

        private static void CreateFirstRunLockFile()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FirstRun.lock");
            File.WriteAllText(filePath, "First run completed");
        }

        private static void SetStartup()
        {
            try
            {
                string appPath = Application.ExecutablePath;
                string appName = "SendAppGUI";
                string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey, true))
                {
                    if (key == null)
                        throw new Exception("Não foi possível acessar o Registro para configurar a inicialização.");

                    key.SetValue(appName, $"\"{appPath}\"");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao configurar inicialização com o Windows: {ex.Message}");
            }
        }
    }
}
