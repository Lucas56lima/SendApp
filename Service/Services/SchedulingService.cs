using Domain.Entities;
using Domain.Interfaces;

namespace Service.Services
{
    public class SchedulingService(ISchedulingRepository repository) : ISchedulingService
    {
        private readonly ISchedulingRepository _repository = repository;
        public Task<IEnumerable<Scheduling>> GetSchedulingAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an existing schedule based on status as parameter.
        /// </summary>
        /// <param name="status"></param>
        /// <returns>An existing Schedling object</returns>
        public async Task<Scheduling> GetSchedulingByStatusAsync(string status)
        {
            var schedulingForDb = await _repository.GetSchedulingByStatusAsync(status);
            if (schedulingForDb == null) 
                return null;

            return schedulingForDb;
        }

        public Task<Scheduling> GetSchedulingByStoreNameAsync(string storeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a new schedule based on parameters where the first day is always 1 and the month is the following month.
        /// </summary>
        /// <param name="scheduling"></param>
        /// <returns>An new Schedling object</returns>
        public async Task<Scheduling> PostSchedulingAsync(Scheduling scheduling)
        {
            int year = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            int nextMonth = DateTime.Now.AddMonths(1).Month;          
           
            if(currentMonth == 12)
            {
                year++;
            }
            DateOnly schandulingDate = new (year, nextMonth, 1);
            scheduling.TransitionDate = schandulingDate;
            scheduling.Status = "Agendado";
            return await _repository.PostSchedulingAsync(scheduling);
        }
    }
}
