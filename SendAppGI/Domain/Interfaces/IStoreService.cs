using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IStoreService
    {
        Task<Store> PostStoreAsync(Store store);
        Task<IEnumerable<Store>> GetStoresAsync();
        Task<Store> GetStoresByNameAsync(string name);
        Task<Store> GetStoresByIdAsync(int id);
        Task<Store> PutStoreByIdAsync(int id, Store store);
    }
}
