using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        public Task<IEnumerable<Log>> GetLogsByDateAsync(int currentMounth)
        {
            throw new NotImplementedException();
        }

        public Task<Log> PostLogAsync(Log log)
        {
            throw new NotImplementedException();
        }
    }
}
