using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IStoreRepository
    {
        Task<Store> PostStoreAsync(Store store);
        Task<IEnumerable<Store>> GetStoresAsync();
        Task<IEnumerable<Store>> GetStoresByNameAsync(string name);
        Task<IEnumerable<Store>> GetStoresByStatusAsync(string status);
    }
}
