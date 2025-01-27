using KayakData.DTOs.EventNS;
using KayakData.ModelsNS;

namespace KayakData.InterfacesNS
{
    public interface IEventRepository
    {
        //public Task<Event[]> GetAllAsync(QueryObject queryObj);
        public Task<List<Event>> GetAllAsync(QueryObject queryObj);
        public Task<Event?> GetByIdAsync(int id);
        public Task<Event> CreateEventAsync(Event eventModel);
        public Task<Event?> UpdateEventAsync(int id, UpdateEventDto eventModel);
        public Task<Event?> DeleteEventAsync(int id);
        public Task<bool> IsEventExist(int id);
    }
}
