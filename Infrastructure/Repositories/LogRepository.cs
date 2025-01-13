using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LogRepository(SendAppContext context) : ILogRepository
    {
        private readonly SendAppContext _context = context;
        public async Task<IEnumerable<Log>> GetLogsByDateAsync(int currentMounth)
        {
            var logs = await _context.Logs
                            .Where(l => l.Created.Month == currentMounth)
                            .ToListAsync();
            return logs;
        }

        public async Task<Log> PostLogAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log;
        }
    }
}
