using Domain.Entities;
using Domain.Interfaces;

namespace Service.Services
{
    public class StoreService : IStoreService
    {
        Task<IEnumerable<Store>> IStoreService.GetStoresAsync()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Store>> IStoreService.GetStoresByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Store>> IStoreService.GetStoresByStatusAsync(string status)
        {
            throw new NotImplementedException();
        }

        Task<Store> IStoreService.PostStoreAsync(Store store)
        {
            throw new NotImplementedException();
        }
    }
}
