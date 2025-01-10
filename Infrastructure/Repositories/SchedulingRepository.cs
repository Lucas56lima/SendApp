using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;

namespace Infrastructure.Repositories
{
    public class SchedulingRepository(SendAppContext context) : ISchedulingRepository
    {
        private readonly SendAppContext _context = context;        

        public Task<IEnumerable<Scheduling>> GetSchedulingAsync()
        {
            throw new NotImplementedException();
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
            Log log = new()
            {
                StoreName = scheduling.Store,
                Message = $"Tarefa Agendada {scheduling.TransitionDate}"
            };
            return scheduling;
        }
    }
}
