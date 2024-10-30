using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KayakTourismWebApi.ControllersNS
{
    [Route("api/eventSubscription")]
    [ApiController]
    public class EventSubscriptionController : ControllerBase
    {
        private readonly UserManager<Customer> _userManager;
        private readonly ApplicationDBContext _context;
        public EventSubscriptionController(UserManager<Customer> userManager, ApplicationDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("apply/{eventId}")]
        [Authorize]
        public async Task<IActionResult> ApplyForEvent([FromRoute]int eventId, string userId)
        {
            //var userId = _userManager.GetUserId(User);
            var customer = await _userManager.FindByIdAsync(userId);
            if (customer == null)
            {
                return Unauthorized("User not found.");
            }

            var @event = await _context.Events.Include(e => e.EventCustomers)
                                     .FirstOrDefaultAsync(e => e.Id == eventId);
            if (@event == null)
            {
                return NotFound("Event not found.");
            }

            if (@event.EventCustomers.Count >= 8)
            {
                @event.IsRegistrationOpenedInt = 0; // close registration
                await _context.SaveChangesAsync();
                return BadRequest("Registration is closed for this event.");
            }

            @event.EventCustomers.Add(new EventCustomer { EventId = @event.Id, CustomerId = customer.Id });
    
            if (@event.EventCustomers.Count >= 8)
            {
                @event.IsRegistrationOpenedInt = 0; // close registration after adding new person
            }

            await _context.SaveChangesAsync();
            return Ok("Successfully applied for the event.");
    }
}
}
