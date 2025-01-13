using Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;

namespace SendAppGI.Services
{
    public class FileService(IConfiguration configuration, MailService mailService, DataStoreService dataStoreService)
    {
        private readonly MailService _mailService = mailService;
        private readonly DataStoreService _dataStoreService = dataStoreService;

        public Store Store { get; private set; }

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
                using (FileStream zipToCreate = new (zipFilePath, FileMode.Create))
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

        public async Task StartWatching(string path, string storeName)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrEmpty(storeName))
            {
                throw new ArgumentException("O caminho e o nome da loja não podem ser nulos ou vazios.");
            }

            Log log = new()
            {
                StoreName = storeName,
                Message = "Iniciando o monitoramento da pasta",
                Created = DateTime.Now,
            };
            await _dataStoreService.PostLogAsync(log);

            FileSystemWatcher watcher = new ()
            {
                Path = path,
                Filter = "*.xml",
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            watcher.Created += OnNewXmlCreated;

            await Task.Delay(Timeout.Infinite);
        }

        public void OnNewXmlCreated(object sender, FileSystemEventArgs e)
        {     
            AddXmlToZip(e.FullPath);
        }

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

                using FileStream zipToOpen = new (zipFilePath, FileMode.OpenOrCreate);
                using ZipArchive archive = new (zipToOpen, ZipArchiveMode.Update);
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar XML ao ZIP: {ex.Message}");
            }
        }

        public static void DeleteZipFile(string path)
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
