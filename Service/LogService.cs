using Domain.Entities;
using Domain.Interfaces;

namespace Service
{
    public class LogService(ILogRepository repository) : ILogService
    {
        private readonly ILogRepository _repository = repository;
        public async Task<IEnumerable<Log>> GetLogsByDateAsync(int currentMounth)
        {
            var logs = await _repository.GetLogsByDateAsync(currentMounth);
            if(logs == null)
                return null;
            return logs;
        }

        public async Task<Log> PostLogAsync(Log log)
        {
            return await _repository.PostLogAsync(log);
        }
    }
}
