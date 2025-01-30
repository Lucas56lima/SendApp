using Domain.Entities;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;

namespace SendAppGI.Services
{
    public class FileService
    {
        private readonly DataStoreService _dataStoreService;
        private CancellationTokenSource cancellationTokenSource;
        private FileSystemWatcher watcher;
        private static readonly object lockObject = new();
        public Store Store;

        public FileService(DataStoreService dataStoreService) => _dataStoreService = dataStoreService;

        public async Task GetFileForPathAsync(string path)
        {
            string appDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

            // Cria um caminho para uma pasta chamada "Data" dentro do diretório da aplicação
            string serviceFilesPath = Path.Combine(appDirectory, "Services", "Files");

            // Verifica se a pasta existe, caso contrário, cria
            if (!Directory.Exists(serviceFilesPath))
            {
                Directory.CreateDirectory(serviceFilesPath);
            }
            string zipFilePath = Path.Combine(serviceFilesPath, $"XML_{DateTime.Now:yyyy_MM}.zip");
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
                       // Verifica se o mês e o ano do arquivo coincidem com o mês e o ano atuais
                       return fileCreation.Month == now.Month && fileCreation.Year == now.Year;
                   })
                    .OrderByDescending(File.GetLastWriteTime) // Ordena para obter o mais recente
                    .ToList();

                if (filterFiles.Count == 0)
                {
                    MessageBox.Show("Nenhum arquivo XML encontrado.");
                    return;
                }

                // Abre o arquivo ZIP
                using var zipToOpen = new FileStream(zipFilePath, FileMode.OpenOrCreate);
                using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

                foreach (var file in filterFiles)
                {
                    string fileName = Path.GetFileName(file);
                    // Verifica se o arquivo já está no ZIP
                    if (archive.GetEntry(fileName) == null)
                    {
                        archive.CreateEntryFromFile(file, fileName);
                    }
                }

                // Pega o último arquivo modificado
                var latestFile = filterFiles.FirstOrDefault();
                if (latestFile == null)
                {
                    return;
                }

                XElement xml = XElement.Load(latestFile); // Usa o caminho completo do arquivo


                var getElements = GetElementsXmls(XElement.Load(latestFile));
                string cnpj = getElements[1];

                string name = getElements[0];


                var storeDb = await _dataStoreService.GetStoreByIdAsync();
                Store store = new()
                {
                    Name = name,
                    Cnpj = cnpj,
                    Email = storeDb.Email,
                    Password = storeDb.Password,
                    Path = storeDb.Path
                };
                // Post executado apenas uma vez
                await _dataStoreService.PutStoreByIdAsync(1, store);
                Log log = new()
                {
                    StoreName = store.Name,
                    Message = "Loja criada!",
                    Created = DateTime.Now
                };
                await _dataStoreService.PostLogAsync(log);
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"Erro de I/O: {ioEx.Message}");
            }
        }


        // Cria um ZIP vazio se o arquivo não existir
        private static void CreateEmptyZipFile(string zipFilePath)
        {
            try
            {
                using (FileStream zipToCreate = new(zipFilePath, FileMode.Create))
                using (new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    // Apenas cria o ZIP vazio
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar o arquivo ZIP: {ex.Message}");
            }
        }

        public static List<string> GetElementsXmls(XElement xml)
        {
            XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
            XNamespace nsc = "http://www.fazenda.sp.gov.br/sat";

            string cnpj = GetElementValue(xml, "CNPJ", ns, nsc);
            string name = GetElementValue(xml, "xNome", ns, nsc);
            return [name, cnpj];
        }

        private static string GetElementValue(XElement xml, string elementName, XNamespace ns, XNamespace nsc)
        {
            return xml.Descendants(ns + "emit").Elements(ns + elementName).FirstOrDefault()?.Value switch
            {
                null or "" => xml.Descendants(nsc + "emit").Elements(nsc + elementName).FirstOrDefault()?.Value switch
                {
                    null or "" => xml.Descendants("emit").Elements(elementName).FirstOrDefault()?.Value ?? "Sem identificação",
                    var value => value
                },
                var value => value
            };
        }
        public async Task StartWatching(string path, string storeName)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(storeName))
            {
                MessageBox.Show("O caminho e o nome da loja não podem ser nulos ou vazios.");
            }

            var storeDb = _dataStoreService.GetStoreByIdAsync();
            if (storeDb != null)
            {
                Store = new Store
                {
                    Path = storeDb.Result.Path,
                    Name = storeDb.Result.Name,
                    Email = storeDb.Result.Email,
                    Password = storeDb.Result.Password
                };
            }
            // Tarefa de segundo plano
            await Task.Run(() =>
            {
                var watcher = new FileSystemWatcher
                {
                    Path = path,
                    Filter = "*.xml",
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };

                watcher.Created += OnNewXmlCreated;

                // Manter o processo ativo
                while (true)
                {
                    Thread.Sleep(1000); // Simula trabalho em segundo plano
                }
            });
        }

        // Método para interromper o monitoramento
        public void StopWatching()
        {
            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
            }

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        public static void OnNewXmlCreated(object sender, FileSystemEventArgs e)
        {
            AddXmlToZip(e.FullPath);
        }
        // Adiciona XML ao ZIP de forma segura
        public static void AddXmlToZip(string path)
        {
            string appDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

            // Cria um caminho para uma pasta chamada "Data" dentro do diretório da aplicação
            string zipFilePath = Path.Combine(appDirectory, "Services", "Files");

            // Verifica se a pasta existe, caso contrário, cria
            if (!Directory.Exists(zipFilePath))
            {
                Directory.CreateDirectory(zipFilePath);
            }

            zipFilePath += $"\\XML_{DateTime.Now:yyyy_MM}.zip";

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
                }
            }
        }


        public void DeleteZipFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar o arquivo: {ex.Message}");
            }
        }

        public async Task ProcessFilesAsync(CancellationToken cancellationToken)
        {
            // Sua lógica para processamento de arquivos
            while (!cancellationToken.IsCancellationRequested)
            {
                await StartWatching(Store.Path, Store.Name);
                // Simulação de tempo de espera ou operação longa
                await Task.Delay(5000, cancellationToken);
            }
        }

    }
}
