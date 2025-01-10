using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILogService
    {
        Task<Log> PostLogAsync(Log log);
        Task<IEnumerable<Log>> GetLogsByDateAsync(int currentMounth);
    }
}
