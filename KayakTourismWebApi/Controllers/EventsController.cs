using KayakTourismWebApi.DataNS;
using Microsoft.AspNetCore.Mvc;

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

            return Ok(theEvent);
        }
    }
}
