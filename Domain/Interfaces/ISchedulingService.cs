using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ISchedulingService
    {
        Task<Scheduling> PostSchedulingAsync(Scheduling scheduling);
        Task<IEnumerable<Scheduling>> GetSchedulingAsync();
        Task<Scheduling> GetSchedulingByStoreNameAsync(string storeName);
        Task<Scheduling> GetSchedulingByStatusAsync(string status);
    }
}
