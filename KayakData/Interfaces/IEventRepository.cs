using KayakData.DTOs.EventNS;
using KayakData.ModelsNS;

namespace KayakData.InterfacesNS
{
    public interface IEventRepository
    {
        Task<Event[]> GetAllAsync(QueryObject queryObj);
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateEventAsync(Event eventModel);
        Task<Event?> UpdateEventAsync(int id, UpdateEventDto eventModel);
        Task<Event?> DeleteEventAsync(int id);
        Task<bool> IsEventExist(int id);
    }
}
