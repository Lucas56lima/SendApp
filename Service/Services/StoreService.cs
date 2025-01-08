using Domain.Entities;
using Domain.Interfaces;

namespace Service.Services
{
    public class StoreService(IStoreRepository repository) : IStoreService
    {
        private readonly IStoreRepository _repository = repository;
        public async Task<Store> GetStoresByIdAsync(int id)
        {
            var storeDb = await _repository.GetStoresByIdAsync(id);
            if (storeDb == null)
                return null;

            return storeDb;
        }

        public async Task<Store> GetStoresByNameAsync(string name)
        {
            var storeDb = await _repository.GetStoresByNameAsync(name);
            if (storeDb == null)
                return null;

            return storeDb;
        }

        public Task<IEnumerable<Store>> GetStoresAsync()
        {
            throw new NotImplementedException();
        }       

        public async Task<Store> PostStoreAsync(Store store)
        {
            ArgumentNullException.ThrowIfNull(store);
            ValidateStrings(store);
            return await _repository.PostStoreAsync(store);
        }

        public static void ValidateStrings(object obj, string invalidValue = "string")
        {
            if (obj == null)
                ArgumentNullException.ThrowIfNull(obj);

            var invalidProperties = obj.GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string)) 
                .Where(p => p.GetValue(obj)?.ToString() == invalidValue) 
                .Select(p => p.Name)
                .ToList();

            if (invalidProperties.Any())
            {
                ArgumentNullException.ThrowIfNull($"The following properties have invalid values: {string.Join(", ", invalidProperties)}");
            }
        }
    }
}
