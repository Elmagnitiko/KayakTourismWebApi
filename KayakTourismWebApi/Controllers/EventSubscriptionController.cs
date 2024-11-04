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

            var customer = await GetCurrentCustomer();

            if (customer == null)
            {
                return Unauthorized("User not found.");
            }

            var @event = await _eventSubscriptionRepository.GetByIdAsync(eventId);
            if (@event == null)
            {
                return NotFound("Event not found.");
            }

            if (@event.IsRegistrationOpened && @event.EventCustomers.Count < 8)
            {
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
            
            return BadRequest("Registration for this event is closed.");
        }

        [HttpPost("closeRegistration/{eventId}")]
        [Authorize]
        //[Authorize(Roles="Moderator")]
        public async Task<IActionResult> CloseRegistration([FromRoute] int eventId)
        {
            var closeEventResult = await _eventSubscriptionRepository.CloseRegistrationAsync(eventId);
            if (closeEventResult == null)
            {
                return NotFound("Event not found.");
            }
            return Ok("Registration closed.");
        }

        [HttpPost("openRegistration/{eventId}")]
        [Authorize]
        //[Authorize(Roles="Moderator")]
        public async Task<IActionResult> OpenRegistration([FromRoute] int eventId)
        {
            var openEventResult = await _eventSubscriptionRepository.OpenRegistrationAsync(eventId);
            if (openEventResult == null)
            {
                return NotFound("Event not found.");
            }
            return Ok("Registration opened.");
        }

        [HttpPost("deleteCustomerFromEvent/{eventId}")]
        [Authorize]
        //[Authorize(Roles="Moderator")]
        public async Task<IActionResult> DeleteCustomerFromEvent([FromRoute] int eventId, string customerId)
        {
            var deletingResult = await _eventSubscriptionRepository.DeleteCustomerFromEvent(eventId, customerId);
            if (deletingResult == null)
            {
                return NotFound("Could not find this event with this customer");
            }

            return Ok("Customer is deleted from the event");
        }

        private async Task<Customer?> GetCurrentCustomer()
        {
            return await _userManager.FindByNameAsync(User.GetUsername());
        }
    }
}
