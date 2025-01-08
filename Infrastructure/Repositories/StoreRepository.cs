using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    public class StoreRepository(SendAppContext context) : IStoreRepository
    {
        private readonly SendAppContext _context = context;
        public Task<IEnumerable<Store>> GetStoresAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Store> GetStoresByIdAsync(int id)
        {
            var storeForDb = await _context.Store
                                .Where(u => u.Id == id)
                                .FirstOrDefaultAsync();
            return storeForDb;
        }       

        public async Task<Store> PostStoreAsync(Store store)
        {
            await _context.Store.AddAsync(store);
            await _context.SaveChangesAsync();
            return store;
        }

        public async Task<Store> GetStoresByNameAsync(string name)
        {
            var storeForDb = await _context.Store
                                .Where(u => u.Name == name)
                                .FirstOrDefaultAsync();
            return storeForDb;
        }
    }
}
