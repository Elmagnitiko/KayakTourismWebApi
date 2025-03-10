using KayakData;
using KayakData.MappersNS;
using KayakData.DTOs.EventNS;
using KayakData.InterfacesNS;
using KayakData.ModelsNS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KayakTourismWebApi.ControllersNS
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepo;
        public EventsController(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject queryObj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var events = await _eventRepo.GetAllAsync(queryObj);
            var eventsDtos = events.Select(e => e.ToEventDto());

            return Ok(eventsDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var theEvent = await _eventRepo.GetByIdAsync(id);

            if (theEvent == null)
            {
                return NotFound();
            }

            return Ok(theEvent.ToEventDto());
        }

        [HttpPost("createEvent")]
        [Authorize(Roles = Constants.ModeratorRole)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEvent = eventDto.ToEventFromCreateEventDto();
            await _eventRepo.CreateEventAsync(createdEvent);
            return CreatedAtAction(nameof(GetById), new {id = createdEvent.Id}, createdEvent);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = Constants.ModeratorRole)]
        public async Task<IActionResult> UpdateEvent([FromRoute] int id, [FromBody] UpdateEventDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventModel = await _eventRepo.UpdateEventAsync(id, eventDto);

            if(eventModel == null)
            {
                return NotFound();
            }

            return Ok(eventModel.ToEventDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Constants.ModeratorRole)]
        public async Task<IActionResult> DeleteEventAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventModel = await _eventRepo.DeleteEventAsync(id);

            if(null == eventModel)
            {
                return NotFound();
            }

            return NoContent(); 
        }
    }
}
