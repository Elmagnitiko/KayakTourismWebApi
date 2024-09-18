using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.DTOsNS;
using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.MappersNS;
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

        public async Task<Event> CreateEventAsync(Event eventModel)
        {
            await _dbContext.Events.AddAsync(eventModel);
            await _dbContext.SaveChangesAsync();
            return eventModel;  
        }

        public async Task<Event?> DeleteEventAsync(int id)
        {
            var eventModel = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventModel == null)
            {
                return null;
            }
            _dbContext.Events.Remove(eventModel);
            await _dbContext.SaveChangesAsync();
            return eventModel;
        }

        public async Task<Event[]> GetAllAsync()
        {
            return await _dbContext.Events.ToArrayAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _dbContext.Events.FindAsync(id);
        }

        public async Task<Event?> UpdateEventAsync(int id, UpdateEventDto eventModel)
        {
            var existingEvent = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEvent == null) 
            {
                return null;
            }

            eventModel.ToEventFromUpdateDto(existingEvent);
            await _dbContext.SaveChangesAsync();

            return existingEvent;
        }

        public Task<bool> IsEventExist(int id)
        {
            throw new NotImplementedException();
        }
    }
}
