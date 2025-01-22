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
        private readonly MailService _mailService;
        public ICommand SaveStoreCommand { get; }
        public ICommand LoadStoreCommand { get; }
        public ICommand LoadSchedulingCommand { get; }
        public ICommand LoadStoreFromCacheCommand { get; }
        public ICommand PutStoreCommand { get; }
        public ICommand StartWatchingCommand { get; }
        public ICommand LoadLogsCommand { get; }
        public ICommand SendEmailCommand { get; }
        public InitialViewModel(DataStoreService service, FileService fileService,MailService mailService)
        {
            _service = service;
            _fileService = fileService;
            _mailService = mailService;
            SaveStoreCommand = new RelayCommand(async () => await SaveStoreCommandAsync(), CanSave);
            LoadStoreCommand = new RelayCommand(async () => await LoadStoreAsync(), CanLoadStore);
            LoadSchedulingCommand = new RelayCommand(async () => await LoadSchedulingAsync(), CanLoadScheduling);
            LoadStoreFromCacheCommand = new RelayCommand(() => LoadStoreFromCacheAsync(), CanLoadStoreCache);
            PutStoreCommand = new RelayCommand(async () => await PutStoreAsync(), CanPutStore);
            StartWatchingCommand = new RelayCommand(async () => await StartWatchingAsync(), CanStartWatching);
            SendEmailCommand = new RelayCommand(async () => await SendEmail(), CanSendEmail);
            LoadLogsCommand = new RelayCommand(async () => await LoadLogsAsync(), CanLoadLogs);
           
            _ = InitializeWatcherAsync();  // Inicia o monitoramento no início da aplicação
        }


        private async Task SendEmail()
        {
            // Aguarde até que o objeto Store esteja disponível
            while (Store == null)
            {
                await Task.Delay(1000);
            }

            string path = "C:\\Users\\lucas\\source\\repos\\Lucas56lima\\SendApp\\Service\\Files\\";
            var zipFiles = Directory.GetFiles(path, "*.zip", SearchOption.AllDirectories);

            if (zipFiles.Length > 0)
            {
                // Aguarde até que o objeto Scheduling esteja disponível
                while (Scheduling == null)
                {
                    await Task.Delay(1000);
                }

                foreach (var file in zipFiles)
                {
                    if (Scheduling.TransitionDate == DateOnly.FromDateTime(DateTime.Now) || Scheduling.Status != null)
                    {
                        // Enviar o e-mail
                        await _mailService.SendMail(Store.Email, Store.Password, file, Store.Name);

                        // Atualizar o agendamento
                        await _service.PutSchedulingByIdAsync(Scheduling.Id, Scheduling);
                        await _service.PostSchedulingByIdAsync(Scheduling);                        
                    }
                }
            }
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

        private Scheduling scheduling;
        public Scheduling Scheduling
        {
            get => scheduling;
            set
            {
                if (scheduling != value)
                {
                    scheduling = value;
                    OnPropertyChanged();
                }
            }
        }

        private async Task LoadSchedulingAsync()
        {
            try
            {
                Scheduling = await _service.GetSchedulingsAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Erro ao carregar o agendamento", ex);
            }
           
        }

        private async Task InitializeWatcherAsync()
        {
            try
            {
                if (Store == null && Scheduling == null)
                {
                    await LoadStoreAsync();
                    await LoadSchedulingAsync();
                }


                if (Scheduling != null)
                {
                    await SendEmail();
                     // Inicia o monitoramento assíncrono
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
                string path = "C:\\Users\\lucas\\Downloads\\Nova pasta";
                await Task.Run(() => _fileService.StartWatching(path,Store.Name));// Inicia o monitoramento em um thread separado
             
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao iniciar observação: {ex.Message}");
            }
        }

        // Método que monitora a pasta usando o FileSystemWatcher        
        
        private bool CanSave() => Store != null && !string.IsNullOrEmpty(Store.Name) && !string.IsNullOrEmpty(Store.Email);
        private bool CanLoadStore() => true;
        private bool CanLoadStoreCache() => true;
        private bool CanPutStore() => true;
        private bool CanStartWatching() => Store != null && !string.IsNullOrEmpty(Store.Path);
        private bool CanLoadLogs() => true;
        private bool CanLoadScheduling() => true;
        private bool CanSendEmail() => true;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
