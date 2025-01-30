using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SchedulingRepository(SendAppContext context) : ISchedulingRepository
    {
        private readonly SendAppContext _context = context;        

        public Task<IEnumerable<Scheduling>> GetSchedulingAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Scheduling> GetSchedulingByIdAsync(int id)
        {
            var schedulingForDb = await _context.Schedulings
                                  .Where(s => s.Id == id)
                                  .FirstOrDefaultAsync();
            return schedulingForDb;
        }

        public async Task<Scheduling> GetSchedulingByStatusAsync(string status)
        {
            var schedulingForDb = await _context.Schedulings
                                 .Where(u => u.Status == status)
                                 .FirstOrDefaultAsync();           

            return schedulingForDb;
        }

        public Task<Scheduling> GetSchedulingByStoreNameAsync(string storeName)
        {
            throw new NotImplementedException();
        }

        public async Task<Scheduling> PostSchedulingAsync(Scheduling scheduling)
        {
            await _context.Schedulings.AddAsync(scheduling);
            await _context.SaveChangesAsync();            
            return scheduling;
        }

        public async Task<Scheduling> PutSchedulingByIdAsync(int id, Scheduling scheduling)
        {
            var schedulingForDb = await GetSchedulingByIdAsync(id);
            _context.Entry(schedulingForDb).CurrentValues.SetValues(scheduling);
            _context.Entry(schedulingForDb).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return scheduling;
        }
    }
}
