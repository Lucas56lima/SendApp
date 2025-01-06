using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        public Task<IEnumerable<Store>> GetStoresAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Store>> GetStoresByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Store>> GetStoresByStatusAsync(string status)
        {
            throw new NotImplementedException();
        }

        public Task<Store> PostStoreAsync(Store store)
        {
            throw new NotImplementedException();
        }
    }
}
