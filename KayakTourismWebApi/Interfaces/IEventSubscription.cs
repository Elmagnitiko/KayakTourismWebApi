using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Mvc;

namespace KayakTourismWebApi.InterfacesNS
{
    public interface IEventSubscription
    {
        Task<Event?> GetByIdAsync(int eventId);
        Task<Event?> CloseRegistrationAsync(int eventId);
        Task<Event?> SubscribeToEventAsync(EventCustomer eventCustomer, int eventId);
        Task<Event?> OpenRegistrationAsync(int eventId);
        Task<EventCustomer?> DeleteCustomerFromEvent(int eventId, string customerId);
        Task<Customer[]> GetAllAppliedCustomersAsync(int eventId);
    }
}
