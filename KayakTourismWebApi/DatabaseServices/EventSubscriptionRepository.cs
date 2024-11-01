using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KayakTourismWebApi.DatabaseServices
{
    public class EventSubscriptionRepository : IEventSubscription
    {
        private readonly ApplicationDBContext _context;
        public EventSubscriptionRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Event?> CloseRegistrationAsync(int eventId)
        {

            var eventModel = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventModel == null) 
            {
                return null;
            }
            eventModel.IsRegistrationOpenedInt = 0;
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<Event?> GetByIdAsync(int eventId)
        {
            return await _context.Events.Include(e => e.EventCustomers)
                                     .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<Event?> SubscribeToEventAsync(EventCustomer eventCustomer, int eventId)
        {
            var eventModel = await _context.Events.Include(e => e.EventCustomers)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventModel == null)
            {
                return null;
            }

            eventModel.EventCustomers.Add(eventCustomer);

            if(eventModel.EventCustomers.Count >= 8)
            {
                eventModel.IsRegistrationOpenedInt = 0;
            }

            await _context.SaveChangesAsync();
            return eventModel;
        }
    }
}
