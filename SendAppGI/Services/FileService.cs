using Domain.Entities;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO.Compression;

namespace SendAppGI.Services
{
    public class FileService(MailService mailService, DataStoreService dataStoreService)
    {
        private readonly MailService _mailService = mailService;
        private readonly DataStoreService _dataStoreService = dataStoreService;
        public Store Store { get; private set; }
        public static void GetFileForPath(string path)
        {
            string fileZip = @"caminho da pasta";

            try
            {
                var filterFiles = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories)
                    .Where(f =>
                    {
                        DateTime fileCreation = File.GetLastWriteTime(f);
                        DateTime now = DateTime.Now;
                        if (now.Month == 1 && fileCreation.Month == 12 && now.Year > fileCreation.Year)
                        {
                            return f.EndsWith(".xml");
                        }

                        return f.EndsWith(".xml")
                               && fileCreation.Month == now.AddMonths(-1).Month;
                    });

                using var archive = ArchiveFactory.Create(ArchiveType.Zip);
                foreach (var file in filterFiles)
                {

                    archive.AddEntry(Path.GetFileName(file), file);
                }

                using var stream = File.OpenWrite(fileZip);
                archive.SaveTo(stream, CompressionType.None);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler arquivos: {ex.Message}"); // Corrigido para exibir a exceção corretamente
            }
        }

        public async Task StartWatching(string path, string storeName)
        {
            Log log = new()
            {
                StoreName = storeName,
                Message = "Iniciando o monitoramento da pasta"
            };
            await _dataStoreService.PostLogAsync(log);

            while (true && !string.IsNullOrWhiteSpace(path))
            {
                FileSystemWatcher watcher = new FileSystemWatcher
                {
                    Path = path,
                    Filter = ".xml",
                    NotifyFilter = NotifyFilters.FileName,
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };
                watcher.Created += OnNewXmlCreated;
                await Task.Delay(3000);
            }
        }

        public void OnNewXmlCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Novo XML detectado: {e.FullPath}");
            AddXmlToZip(e.FullPath);
        }

        public static void AddXmlToZip(string filePath)
        {
            try
            {
                // Atualiza o nome do ZIP caso tenha mudado o mês
                UpdateZipFileName(filePath);

                // Adiciona o XML ao ZIP
                using FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate);
                using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
                string fileName = Path.GetFileName(filePath);

                // Verifica se o arquivo já está no ZIP
                if (archive.GetEntry(fileName) == null)
                {
                    archive.CreateEntryFromFile(filePath, fileName);
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

        public static void UpdateZipFileName(string path)
        {
            // Define o nome do arquivo ZIP baseado no mês atual
            string zipFileName = $"XML_{DateTime.Now:yyyy_MM}.zip";
            path = Path.Combine(path, zipFileName);

            if (!File.Exists(path) && DateTime.Now.Day == 1)
            {
                Console.WriteLine($"Criando novo arquivo ZIP: {path}");
                using (FileStream zipToCreate = new FileStream(path, FileMode.Create))
                using (new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    // Apenas cria o ZIP vazio
                }
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
                // Trata erros, como problemas de permissões
                Console.WriteLine($"Erro ao deletar o arquivo: {ex.Message}");
            }
        }
    }
}
