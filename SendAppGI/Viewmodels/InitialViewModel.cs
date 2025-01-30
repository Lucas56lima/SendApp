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
        public ICommand LoadSchedulingCommand { get; }
        public ICommand LoadStoreFromCacheCommand { get; }
        public ICommand PutStoreCommand { get; }        
        public ICommand LoadLogsCommand { get; }
        public ICommand SendEmailCommand { get; }
        public ICommand GetFirstArchiveCommand { get; }
        public ICommand SaveLogCommand { get; }
        public ICommand PutSchedulingCommand { get; }
        public InitialViewModel(DataStoreService service, FileService fileService)
        {
            _service = service;
            _fileService = fileService;            
            SaveStoreCommand = new RelayCommand(async () => await SaveStoreAsync(), CanSave);
            LoadStoreCommand = new RelayCommand(async () => await LoadStoreAsync(), CanLoadStore);
            LoadSchedulingCommand = new RelayCommand(async () => await LoadSchedulingAsync(), CanLoadScheduling);
            LoadStoreFromCacheCommand = new RelayCommand(() => LoadStoreFromCacheAsync(), CanLoadStoreCache);
            PutStoreCommand = new RelayCommand(async () => await PutStoreAsync(), CanPutStore);                  
            LoadLogsCommand = new RelayCommand(async () => await LoadLogsAsync(), CanLoadLogs);
            GetFirstArchiveCommand = new RelayCommand(async () => await GetFirstArchive(), CanGetFirstArchive);
            
        }
        private async Task GetFirstArchive()
        {
            if(Store != null)
            {
                await _fileService.GetFileForPathAsync(Store.Path);
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
                MessageBox.Show($"Erro ao carregar o agendamento {ex}");
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
                MessageBox.Show($"Erro ao carregar logs: {ex.Message}");
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
                MessageBox.Show($"Erro ao carregar store: {ex.Message}");
            }
        }

        private async Task SaveStoreAsync()
        {
            try
            {
                if (Store != null)
                {
                    bool success = await _service.PostStoreAsync(Store);
                    if (success)
                    {
                        OnPropertyChanged(nameof(Store));                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar store: {ex.Message}");
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
                MessageBox.Show($"Erro ao carregar store do cache: {ex.Message}");
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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar store: {ex.Message}");
            }
        }

        

        // Método que monitora a pasta usando o FileSystemWatcher        
        
        private bool CanSave() => Store != null && !string.IsNullOrEmpty(Store.Name) && !string.IsNullOrEmpty(Store.Email);
        private bool CanLoadStore() => true;
        private bool CanLoadStoreCache() => true;
        private bool CanPutStore() => true;        
        private bool CanLoadLogs() => true;
        private bool CanLoadScheduling() => true;        
        private bool CanGetFirstArchive() => true;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
