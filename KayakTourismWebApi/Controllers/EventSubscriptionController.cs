using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.HelpersNS;
using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KayakTourismWebApi.ControllersNS
{
    [Route("api/eventSubscription")]
    [ApiController]
    public class EventSubscriptionController : ControllerBase
    {
        private readonly UserManager<Customer> _userManager;
        private readonly IEventSubscription _eventSubscriptionRepository;
        public EventSubscriptionController(UserManager<Customer> userManager, IEventSubscription eventSubsRepo)
        {
            _userManager = userManager;
            _eventSubscriptionRepository = eventSubsRepo;
        }

        [HttpPost("apply/{eventId}")]
        [Authorize]
        public async Task<IActionResult> ApplyForEvent([FromRoute]int eventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerName = User.Getusername();
            var customer = await _userManager.FindByNameAsync(customerName);

            if (customer == null)
            {
                return Unauthorized("User not found.");
            }

            var @event = await _eventSubscriptionRepository.GetByIdAsync(eventId);
            if (@event == null)
            {
                return NotFound("Event not found.");
            }

            if (@event.EventCustomers.Count >= 8)
            {
                var closeEventResult = await _eventSubscriptionRepository.CloseRegistrationAsync(eventId);
                if(closeEventResult == null)
                {
                    return NotFound("Event not found.");
                }
                return BadRequest("Registration is closed for this event.");
            }

            var isAlreadySubscribed = @event.EventCustomers.Exists(ec => ec.CustomerId == customer.Id);
            if (isAlreadySubscribed)
            {
                return BadRequest("User is already subscribed to this event.");
            }

            var eventSubscriptionResult = await _eventSubscriptionRepository.SubscribeToEventAsync(new EventCustomer { EventId = @event.Id, CustomerId = customer.Id }, eventId);

            if (eventSubscriptionResult == null)
            {
                return NotFound("Event not found.");
            }

            return Ok("Successfully applied for the event.");
        }
    }
}
