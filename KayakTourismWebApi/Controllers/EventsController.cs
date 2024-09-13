using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.DTOsNS;
using KayakTourismWebApi.MappersNS;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetAll()
        {
            var events = _dbContext.Events.ToArray();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var theEvent = _dbContext.Events.Find(id);

            if (theEvent == null)
            {
                return NotFound();
            }

            return Ok(theEvent.ToEventDto());
        }

        [HttpPost]
        //[Authorize(Role="Moderator")]
        public IActionResult CreateEvent([FromBody] CreateEventDto eventDto)
        {
            var createdEvent = eventDto.ToEventFromCreateEventDto();
            _dbContext.Events.Add(createdEvent);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetById), new {id = createdEvent.Id}, createdEvent);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(Role="Moderator")]
        public IActionResult UpdateEvent([FromRoute] int id, [FromBody] UpdateEventDto eventDto)
        {
            var eventModel = _dbContext.Events.FirstOrDefault(x => x.Id == id);

            if(eventModel == null)
            {
                return NotFound();
            }

            eventDto.ToEventFromUpdateDto(eventModel);
            _dbContext.SaveChanges();
            return Ok(eventModel.ToEventDto());
        }
    }
}
