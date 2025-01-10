using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace SendAppGI.Services
{
    public class DataStoreService(IMemoryCache cache, HttpClient client)
    {
        private readonly IMemoryCache memoryCache = cache;
        private readonly HttpClient httpClient = client;
        private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(30);


        public async Task<Store> GetStoreByIdAsync()
        {
            const string cacheKey = "StoreById";

            if (memoryCache.TryGetValue(cacheKey, out Store cachedStore))
                return cachedStore;

            var storeDb = await httpClient.GetFromJsonAsync<Store>("https://localhost:7185/api/Store/GetStoreByIdAsync?id=1");
            if (storeDb == null)
            {
                MessageBox.Show("Erro ao obter dados da loja.");
                return null;
            }

            // Armazena no cache com duração especificada.
            memoryCache.Set(cacheKey, storeDb, cacheDuration);

            return storeDb;
        }

        public async Task<bool> PostStoreAsync(Store store)
        {
            if (store != null)
            {
                using HttpResponseMessage response = await httpClient.PostAsJsonAsync("https://localhost:7185/api/Store/PostStoreAsync", store);
                return response.IsSuccessStatusCode;
            }
            return false;
        }

        public async Task<Store> GetFromCacheAsync()
        {
            var store = memoryCache.Get("StoreById") as Store;
            if (store == null)
                Console.WriteLine("Dados não encontrados no cachê");
            return store;
        }

        public async Task<bool>PutStoreByIdAsync(int id, Store store)
        {
            var storeByCache = await GetFromCacheAsync();
            if (storeByCache == null)
                return false;

            using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"https://localhost:7185/api/Store/PutStoreByIdAsync?id={id}", store);

            if(response.IsSuccessStatusCode)
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
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync("https://localhost:7185/api/Store/PostLogAsync", log);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

    }
}
