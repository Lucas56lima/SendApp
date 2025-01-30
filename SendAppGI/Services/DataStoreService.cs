using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace SendAppGI.Services
{
    public class DataStoreService(IMemoryCache cache, 
        IStoreService storeService,
        ISchedulingService schedulingService,
        ILogService logService)
    {
        private readonly IMemoryCache memoryCache = cache;
        private readonly IStoreService _storeService = storeService;
        private readonly ISchedulingService _schedulingService = schedulingService;
        private readonly ILogService _logService = logService;
        private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(30);


        public async Task<Store> GetStoreByIdAsync()
        {
            const string cacheKey = "StoreById";

            if (memoryCache.TryGetValue(cacheKey, out Store cachedStore))
                return cachedStore;

            var storeDb = await _storeService.GetStoresByIdAsync(1);
            if (storeDb == null)
            {
                MessageBox.Show("Erro ao obter dados da loja.");
                return null;
            }

            // Armazena no cache com duração especificada.
            memoryCache.Set(cacheKey, storeDb, cacheDuration);
            if (!IsFirstRunCompleted())
            {
                Scheduling scheduling = new()
                {
                    Store = storeDb.Name
                };
                await PostSchedulingByIdAsync(scheduling);
                MarkAsCompleted();
            }
                        
            return storeDb;
        }

        public async Task<bool> PostStoreAsync(Store store)
        {
            if (store != null)
            {
                await _storeService.PostStoreAsync(store);
               
                return true;
            }
            return false;
        }

        public Store GetFromCache()
        {
            var store = memoryCache.Get("StoreById") as Store;                           
            return store;
        }

        public async Task<bool>PutStoreByIdAsync(int id, Store store)
        {
            var storeByCache = GetFromCache();
            if (storeByCache == null)
                return false;

            var putStore = await _storeService.PutStoreByIdAsync(id, store);

            if(putStore != null)
            {
                MessageBox.Show("Dados atualizados com sucesso");
                await GetStoreByIdAsync();
                return true;
            }

            MessageBox.Show("Erro ao atualizar dados.");
            return false;
        }

        public async Task<bool> PostLogAsync(Log log)
        {
            if (log != null)
            {
                await _logService.PostLogAsync(log);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Log>> GetLogsByDateAsync(int currentMonth)
        {
            //const string cacheKey = "Log";
            
            var logs = await _logService.GetLogsByDateAsync(currentMonth);
            if (logs != null)
                return logs;

            MessageBox.Show("Erro ao carregar os logs");
            return null;
        }

        public async Task<Scheduling>GetSchedulingsAsync()
        {
            var scheduling = await _schedulingService.GetSchedulingByStatusAsync("Agendado");
            if (scheduling != null)
                return scheduling;
            return null;
        }
        public async Task<bool> PutSchedulingByIdAsync(int id, Scheduling scheduling)
        {
         
            var putScheduling = await _schedulingService.PutSchedulingByIdAsync(id, scheduling);
            if(putScheduling != null)
                return true;
            
            return false;
        }
        public async Task<bool> PostSchedulingByIdAsync(Scheduling scheduling)
        {
            var postScheduling = await _schedulingService.PostSchedulingAsync(scheduling);
            if (postScheduling != null)                            
                return true;
            
            return false;
        }

        private void MarkAsCompleted()
        {
            // Cria um arquivo indicando que o formulário já foi exibido
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FirstPostScheduling.lock");
            File.WriteAllText(filePath, "completed");
        }

        private static bool IsFirstRunCompleted()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FirstPostScheduling.lock");

            // Verifica se o arquivo existe
            return File.Exists(filePath);
        }

    }
}
