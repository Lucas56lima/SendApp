using Domain.Entities;
using Domain.Interfaces;

namespace Service.Services
{
    public class SchedulingService : ISchedulingService
    {
        public Task<IEnumerable<Scheduling>> GetSchedulingAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Scheduling>> GetSchedulingByStoreNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Scheduling> PostSchedilingAsync(Scheduling scheduling)
        {
            throw new NotImplementedException();
        }
    }
}
