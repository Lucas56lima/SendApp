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
        public ICommand SaveStoreCommand { get; }
        public ICommand LoadStoreCommand { get; }
        public ICommand LoadStoreFromCacheCommand { get; }

        public InitialViewModel(DataStoreService service)
        {
            storeService = service;
            SaveStoreCommand = new RelayCommand(async () => await Save(), CanSave);
            LoadStoreCommand = new RelayCommand(async () => await LoadStore(), CanLoadStore);
            LoadStoreFromCacheCommand = new RelayCommand(async () => await LoadStoreCache(), CanLoadStoreCache);
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
            if (Store == null)
                Store = await storeService.GetFromCacheAsync();
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
