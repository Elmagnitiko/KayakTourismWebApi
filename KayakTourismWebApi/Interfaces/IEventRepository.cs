using KayakTourismWebApi.DTOsNS;
using KayakTourismWebApi.ModelsNS;

namespace KayakTourismWebApi.InterfacesNS
{
    public interface IEventRepository
    {
        Task<Event[]> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateEventAsync(Event eventModel);
        Task<Event?> UpdateEventAsync(int id, UpdateEventDto eventModel);
        Task<Event?> DeleteEventAsync(int id);
        Task<bool> IsEventExist(int id);
    }
}
