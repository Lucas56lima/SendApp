using Domain.Entities;
using SendAppGI.Commands;
using SendAppGI.Services;
using System.ComponentModel;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SendAppGI.Viewmodels
{
    public class InitialViewModel : INotifyPropertyChanged
    {
        private readonly DataStoreService _service;
        private readonly FileService _fileService;
        public ICommand SaveStoreCommand { get; }
        public ICommand LoadStoreCommand { get; }
        public ICommand LoadStoreFromCacheCommand { get; }
        public ICommand PutStoreCommand { get; }
        public ICommand StartWatchingCommand { get; }
        public ICommand LoadLogsCommand { get; }

        public InitialViewModel(DataStoreService service, FileService fileService)
        {
            _service = service;
            _fileService = fileService;

            SaveStoreCommand = new RelayCommand(async () => await SaveStoreCommandAsync(), CanSave);
            LoadStoreCommand = new RelayCommand(async () => await LoadStoreAsync(), CanLoadStore);
            LoadStoreFromCacheCommand = new RelayCommand(() => LoadStoreFromCacheAsync(), CanLoadStoreCache);
            PutStoreCommand = new RelayCommand(async () => await PutStoreAsync(), CanPutStore);
            StartWatchingCommand = new RelayCommand(async () => await StartWatchingAsync(), CanStartWatching);

            LoadLogsCommand = new RelayCommand(async () => await LoadLogsAsync(), CanLoadLogs);
            _ = InitializeWatcherAsync();  // Inicia o monitoramento no início da aplicação
        }

        private Store store;
        public Store Store
        {
            get => store;
            set
            {
                if (store != value)
                {
                    store = value;
                    OnPropertyChanged();
                }
            }
        }

        private IEnumerable<Log> logs;
        public IEnumerable<Log> Logs
        {
            get => logs;
            set
            {
                if (logs != value)
                {
                    logs = value;
                    OnPropertyChanged();
                }
            }
        }

        private async Task InitializeWatcherAsync()
        {
            try
            {
                if (Store == null)
                {
                    await LoadStoreAsync();
                }

                if (Store != null && !string.IsNullOrEmpty(Store.Path) && !string.IsNullOrEmpty(Store.Name))
                {
                    await StartWatchingAsync(); // Inicia o monitoramento assíncrono
                }
                else
                {
                    Console.WriteLine("Store não está disponível ou possui dados inválidos para iniciar o monitoramento.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o monitoramento: {ex.Message}");
            }
        }

        private async Task LoadLogsAsync()
        {
            try
            {
                int currentMonth = DateTime.Now.Month;
                Logs = await _service.GetLogsByDateAsync(currentMonth);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar logs: {ex.Message}");
            }
        }

        private async Task LoadStoreAsync()
        {
            try
            {
                Store = await _service.GetStoreByIdAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar store: {ex.Message}");
            }
        }

        private async Task SaveStoreCommandAsync()
        {
            try
            {
                if (Store != null)
                {
                    bool success = await _service.PostStoreAsync(Store);
                    if (success)
                    {
                        OnPropertyChanged(nameof(Store));
                        Console.WriteLine("Store salva com sucesso.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar store: {ex.Message}");
            }
        }

        private void LoadStoreFromCacheAsync()
        {
            try
            {
                Store ??= _service.GetFromCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar store do cache: {ex.Message}");
            }
        }

        private async Task PutStoreAsync()
        {
            try
            {
                if (Store != null)
                {
                    bool success = await _service.PutStoreByIdAsync(Store.Id, Store);
                    if (success)
                    {
                        OnPropertyChanged(nameof(Store));
                        Console.WriteLine("Store atualizada com sucesso.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar store: {ex.Message}");
            }
        }

        private async Task StartWatchingAsync()
        {
            try
            {
                if (Store != null && !string.IsNullOrEmpty(Store.Path) && !string.IsNullOrEmpty(Store.Name))
                {
                    // Aqui, iniciamos o FileSystemWatcher de forma assíncrona
                    await Task.Run(() => WatchDirectory(Store.Path, Store.Name)); // Inicia o monitoramento em um thread separado
                }
                else
                {
                    Console.WriteLine("Store ou caminho inválido para começar a observar.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao iniciar observação: {ex.Message}");
            }
        }

        // Método que monitora a pasta usando o FileSystemWatcher
        private void WatchDirectory(string path, string storeName)
        {
            try
            {
                FileSystemWatcher watcher = new FileSystemWatcher
                {
                    Path = path,
                    Filter = "*.xml", // Defina o tipo de arquivo que você quer observar
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite, // Observar alterações no nome e escrita
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true // Inclui subdiretórios
                };

                watcher.Created += (sender, e) =>
                {
                    // O que fazer quando um arquivo for criado
                    Console.WriteLine($"Arquivo criado: {e.FullPath}");
                    AddXmlToZip(e.FullPath); // Função para adicionar o arquivo ao ZIP
                };

                watcher.Changed += (sender, e) =>
                {
                    // O que fazer quando um arquivo for alterado
                    Console.WriteLine($"Arquivo alterado: {e.FullPath}");
                    AddXmlToZip(e.FullPath); // Função para adicionar o arquivo ao ZIP
                };

                // Mantém o processo de monitoramento ativo
                while (true)
                {
                    Task.Delay(1000).Wait(); // Faz uma pausa entre as iterações
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao configurar o FileSystemWatcher: {ex.Message}");
            }
        }

        // Função para adicionar o arquivo XML ao ZIP
        private void AddXmlToZip(string path)
        {
            try
            {
                string zipFilePath = "C:\\path_to_your_zip\\file.zip";
                using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.OpenOrCreate))
                using (var archive = new System.IO.Compression.ZipArchive(zipToOpen, System.IO.Compression.ZipArchiveMode.Update))
                {
                    string fileName = Path.GetFileName(path);
                    archive.CreateEntryFromFile(path, fileName);
                    Console.WriteLine($"Arquivo {fileName} adicionado ao ZIP.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar o XML ao ZIP: {ex.Message}");
            }
        }

        private bool CanSave() => Store != null && !string.IsNullOrEmpty(Store.Name) && !string.IsNullOrEmpty(Store.Email);
        private bool CanLoadStore() => true;
        private bool CanLoadStoreCache() => true;
        private bool CanPutStore() => true;
        private bool CanStartWatching() => Store != null && !string.IsNullOrEmpty(Store.Path);
        private bool CanLoadLogs() => true;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
