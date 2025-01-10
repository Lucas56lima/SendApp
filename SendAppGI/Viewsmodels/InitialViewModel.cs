using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using SendAppGI.Commands;
using SendAppGI.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SendAppGI.Viewsmodels
{
    public class InitialViewModel : INotifyPropertyChanged
    {
        private readonly DataStoreService storeService;
        private readonly FileService _fileService;
        public ICommand SaveStoreCommand { get; }
        public ICommand LoadStoreCommand { get; }
        public ICommand LoadStoreFromCacheCommand { get; }
        public ICommand PutStoreCommand { get; }
        public ICommand StartWatchingCommand { get; }

        public InitialViewModel(DataStoreService service, FileService fileService)
        {
            storeService = service;
            _fileService = fileService; 
            SaveStoreCommand = new RelayCommand(async () => await Save(), CanSave);
            LoadStoreCommand = new RelayCommand(async () => await LoadStore(), CanLoadStore);
            LoadStoreFromCacheCommand = new RelayCommand(async () => await LoadStoreCache(), CanLoadStoreCache);
            PutStoreCommand = new RelayCommand(async () => await PutStore(), CanPutStore);
            StartWatchingCommand = new RelayCommand(async () => await StartWatching(), CanStartWatching);
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

        private async Task LoadStoreAsync()
        {
            Store = await storeService.GetStoreByIdAsync();
        }

        private async Task SaveStoreAsync()
        {
            if (Store != null)
            {
                bool success = await storeService.PostStoreAsync(Store);
                if (success)
                    OnPropertyChanged(nameof(Store)); // Atualiza a interface.
            }
        }
         
        private async Task LoadStoreFromCacheAsync()
        {
            Store ??= await storeService.GetFromCacheAsync();
        }

        private async Task Save()
        {
            try
            {
                await SaveStoreAsync();
            }
            catch (Exception ex)
            {
                // Aqui você pode exibir um feedback para o usuário
                Console.WriteLine($"Erro ao salvar: {ex.Message}");
                // Por exemplo, exibir uma mensagem de erro na UI
            }
        }

        private bool CanSave()
        {
            // Validar se é possível salvar
            return Store != null && !string.IsNullOrEmpty(Store.Name) && !string.IsNullOrEmpty(Store.Email);
        }

        private async Task LoadStore()
        {
            try
            {
                await LoadStoreAsync();
            }
            catch (Exception ex)
            {
                // Tratar exceções de carregamento de dados
                Console.WriteLine($"Erro ao carregar: {ex.Message}");
                // Exiba uma mensagem de erro para o usuário, se necessário
            }
        }

        private bool CanLoadStore()
        {
            // Validação se é possível carregar os dados (exemplo de verificação simples)
            return true;
        }
        private async Task LoadStoreCache()
        {
            try
            {
                await LoadStoreFromCacheAsync();
            }
            catch (Exception ex)
            {
                // Tratar exceções de carregamento de dados
                Console.WriteLine($"Erro ao carregar: {ex.Message}");
                // Exiba uma mensagem de erro para o usuário, se necessário
            }
        }

        private bool CanLoadStoreCache()
        {
            // Validação se é possível carregar os dados (exemplo de verificação simples)
            return true;
        }


        private async Task PutStoreById()
        {
            bool success = await storeService.PutStoreByIdAsync(Store.Id,Store);
            if (success)
                OnPropertyChanged(nameof(Store));
        }

        private bool CanPutStore()
        {
            return true;
        }

        private async Task PutStore()
        {
            try
            {
                await PutStoreById();
            }
            catch(Exception ex)
            {
                // Tratar exceções de carregamento de dados
                Console.WriteLine($"Erro ao atualizar: {ex.Message}");
                // Exiba uma mensagem de erro para o usuário, se necessário
            }
        }

        private bool CanStartWatching()
        {
            return true;
        }

        private async Task StartWatching()
        {
            if(Store != null)
                await _fileService.StartWatching(Store.Path, Store.Name);            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
