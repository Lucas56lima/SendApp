using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILogRepository
    {
        Task<Log> PostLogAsync(Log log);
        Task<IEnumerable<Log>> GetLogsByDateAsync(int currentMounth);
    }
}
