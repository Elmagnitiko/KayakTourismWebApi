using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.DTOsNS;
using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.EntityFrameworkCore;

namespace KayakTourismWebApi.DatabaseServicesNS
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public EventRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public Task<Event> CreateEventAsync(Event eventModel)
        {
            throw new NotImplementedException();
        }

        public Task<Event?> DeleteEventAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Event[]> GetAllAsync()
        {
            return await _dbContext.Events.ToArrayAsync();
        }

        public Task<Event?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEventExist(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Event?> UpdateEventAsync(int id, UpdateEventDto eventModel)
        {
            throw new NotImplementedException();
        }
    }
}
