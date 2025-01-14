using Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;

namespace SendAppGI.Services
{
    public class FileService
    {
        private readonly MailService _mailService;
        private readonly DataStoreService _dataStoreService;

        private static readonly object lockObject = new();

        public Store Store { get; private set; }

        public FileService(IConfiguration configuration, MailService mailService, DataStoreService dataStoreService)
        {
            _mailService = mailService;
            _dataStoreService = dataStoreService;
        }

        public void GetFileForPath(string path)
        {
            string filePath = "C:\\Users\\Usuário\\source\\repos\\SendApp\\Service\\Files\\";
            string zipFilePath = Path.Combine(filePath, $"XML_{DateTime.Now:yyyy_MM}.zip");

            try
            {
                // Verifica se o arquivo ZIP já existe. Se não, cria um vazio.
                if (!File.Exists(zipFilePath))
                {
                    CreateEmptyZipFile(zipFilePath);
                }

                var filterFiles = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories)
                    .Where(f =>
                    {
                        DateTime fileCreation = File.GetLastWriteTime(f);
                        DateTime now = DateTime.Now;
                        return (now.Month == 1 && fileCreation.Month == 12 && now.Year > fileCreation.Year) ||
                               (fileCreation.Month == now.AddMonths(-1).Month);
                    });

                using var zipToOpen = new FileStream(zipFilePath, FileMode.OpenOrCreate);
                using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

                // Adiciona os arquivos XML ao ZIP
                foreach (var file in filterFiles)
                {
                    string fileName = Path.GetFileName(file);
                    // Verifica se o arquivo já está no ZIP
                    if (archive.GetEntry(fileName) == null)
                    {
                        archive.CreateEntryFromFile(file, fileName);
                        Console.WriteLine($"Arquivo {fileName} adicionado ao ZIP.");
                    }
                    else
                    {
                        Console.WriteLine($"Arquivo {fileName} já está no ZIP.");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler arquivos: {ex.Message}");
            }
        }

        // Cria um ZIP vazio se o arquivo não existir
        private static void CreateEmptyZipFile(string zipFilePath)
        {
            try
            {
                Console.WriteLine($"Criando novo arquivo ZIP: {zipFilePath}");
                using (FileStream zipToCreate = new(zipFilePath, FileMode.Create))
                using (new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    // Apenas cria o ZIP vazio
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar o arquivo ZIP: {ex.Message}");
            }
        }

        public void StartWatching(string path, string storeName)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrEmpty(storeName))
            {
                throw new ArgumentException("O caminho e o nome da loja não podem ser nulos ou vazios.");
            }

            // Configuração do FileSystemWatcher
            var watcher = new FileSystemWatcher
            {
                Path = path,
                Filter = "*.xml",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            watcher.Created += OnNewXmlCreated;

            // Use uma nova thread para não bloquear a execução
            Task.Run(() =>
            {
                Console.WriteLine("Iniciando o monitoramento de arquivos...");
                // Mantenha o monitoramento ativo, fazendo com que o código não saia
                while (true)
                {
                    Thread.Sleep(1000);
                }
            });
        }

        public static void OnNewXmlCreated(object sender, FileSystemEventArgs e)
        {
            // Garantir que o código abaixo seja executado na thread principal
            if (e is FileSystemEventArgs fileEventArgs)
            {
                // Exemplo de como invocar a atualização da UI na thread principal
                Application.OpenForms[0].Invoke(new Action(() =>
                {
                    // Atualizar a UI aqui, por exemplo:
                    MessageBox.Show($"Novo arquivo encontrado: {fileEventArgs.FullPath}");
                }));

                AddXmlToZip(fileEventArgs.FullPath);
            }
        }
        // Adiciona XML ao ZIP de forma segura
        public static void AddXmlToZip(string path)
        {
            string filePath = "C:\\Users\\Usuário\\source\\repos\\SendApp\\Service\\Files\\";
            string zipFilePath = Path.Combine(filePath, $"XML_{DateTime.Now:yyyy_MM}.zip");

            try
            {
                // Se o arquivo ZIP não existir, cria um vazio
                if (!File.Exists(zipFilePath))
                {
                    CreateEmptyZipFile(zipFilePath);
                }

                // Adiciona um bloqueio para evitar concorrência no acesso ao arquivo ZIP
                lock (lockObject)
                {
                    using FileStream zipToOpen = new(zipFilePath, FileMode.OpenOrCreate);
                    using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Update);
                    string fileName = Path.GetFileName(path);

                    // Verifica se o arquivo já está no ZIP
                    if (archive.GetEntry(fileName) == null)
                    {
                        archive.CreateEntryFromFile(path, fileName);
                        Console.WriteLine($"Arquivo {fileName} adicionado ao ZIP.");
                    }
                    else
                    {
                        Console.WriteLine($"Arquivo {fileName} já está no ZIP.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar XML ao ZIP: {ex.Message}");
            }
        }

        public void DeleteZipFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Console.WriteLine("Arquivo deletado com sucesso");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar o arquivo: {ex.Message}");
            }
        }
    }
}
