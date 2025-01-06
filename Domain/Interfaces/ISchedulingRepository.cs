using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ISchedulingRepository
    {
        Task<Scheduling> PostSchedilingAsync(Scheduling scheduling);
        Task<IEnumerable<Scheduling>> GetSchedulingAsync();
        Task<IEnumerable<Scheduling>> GetSchedulingByStoreNameAsync(string name);        
    }
}
