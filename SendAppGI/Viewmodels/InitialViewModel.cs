using Domain.Entities;
using SendAppGI.Commands;
using SendAppGI.Services;
using System.ComponentModel;
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
            LoadStoreFromCacheCommand = new RelayCommand(async () => await LoadStoreFromCacheAsync(), CanLoadStoreCache);
            PutStoreCommand = new RelayCommand(async () => await PutStoreAsync(), CanPutStore);
            StartWatchingCommand = new RelayCommand(async () =>
            {
                string pathTest = "C:\\Users\\Usuário\\Desktop\\Nova pasta";
                if (Store != null && !string.IsNullOrEmpty(pathTest) && !string.IsNullOrEmpty(Store.Name))
                {
                    await StartWatchingAsync(pathTest, Store.Name);
                }
                else
                {
                    Console.WriteLine("Store ou caminho inválido para começar a observar.");
                }
            }, CanStartWatching);

            LoadLogsCommand = new RelayCommand(async () => await LoadLogsAsync(), CanLoadLogs);
            _ = InitializeWatcherAsync();
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
                    await StartWatchingAsync(Store.Path, Store.Name);
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

        private async Task LoadStoreFromCacheAsync()
        {
            try
            {
                Store ??= await _service.GetFromCacheAsync();
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

        private async Task StartWatchingAsync(string path, string storeName)
        {
            try
            {
                if (Store != null && !string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(storeName))
                {
                    await _fileService.StartWatching(path, storeName);
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

        private bool CanSave() => Store != null && !string.IsNullOrEmpty(Store.Name) && !string.IsNullOrEmpty(Store.Email);
        private bool CanLoadStore() => true;
        private bool CanLoadStoreCache() => true;
        private bool CanPutStore() => true;
        private bool CanStartWatching() => true;
        private bool CanLoadLogs() => true;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
