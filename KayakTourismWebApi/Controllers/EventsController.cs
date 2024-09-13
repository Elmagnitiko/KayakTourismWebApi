using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.DTOsNS;
using KayakTourismWebApi.MappersNS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace KayakTourismWebApi.ControllersNS
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        public EventsController(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var events = await _dbContext.Events.ToArrayAsync();
            var eventsDtos = events.Select(e => e.ToEventDto());

            return Ok(eventsDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute]int id)
        {
            var theEvent = await _dbContext.Events.FindAsync(id);

            if (theEvent == null)
            {
                return NotFound();
            }

            return Ok(theEvent.ToEventDto());
        }

        [HttpPost]
        //[Authorize(Role="Moderator")]
        public async Task<IActionResult> CreateEventAsync([FromBody] CreateEventDto eventDto)
        {
            var createdEvent = eventDto.ToEventFromCreateEventDto();
            await _dbContext.Events.AddAsync(createdEvent);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByIdAsync), new {id = createdEvent.Id}, createdEvent);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(Role="Moderator")]
        public async Task<IActionResult> UpdateEventAsync([FromRoute] int id, [FromBody] UpdateEventDto eventDto)
        {
            var eventModel = await _dbContext.Events
                .FirstOrDefaultAsync(x => x.Id == id);

            if(eventModel == null)
            {
                return NotFound();
            }

            eventDto.ToEventFromUpdateDto(eventModel);
            await _dbContext.SaveChangesAsync();
            return Ok(eventModel.ToEventDto());
        }

        [HttpDelete]
        [Route("{id}")]
        //[Authorize(Role="Moderator")]
        public async Task<IActionResult> DeleteEventAsync([FromRoute] int id)
        {
            var eventModel = await _dbContext.Events
                .FirstOrDefaultAsync(x => x.Id == id);

            if(null == eventModel)
            {
                return NotFound();
            }

            _dbContext.Events.Remove(eventModel);
            await _dbContext.SaveChangesAsync();

            return NoContent(); 
        }

    }
}
