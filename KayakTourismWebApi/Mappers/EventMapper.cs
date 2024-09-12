using KayakTourismWebApi.DTOsNS;
using KayakTourismWebApi.ModelsNS;

namespace KayakTourismWebApi.MappersNS
{
    public static class EventMapper
    {
        public static Event ToEventFromCreateEventDto(this CreateEventDto createEventDto)
        {
            return new Event
            {
                Name = createEventDto.Name,
                Description = createEventDto.Description,
                Price = createEventDto.Price,
                EventStarts = createEventDto.EventStarts,
                EventEnds = createEventDto.EventEnds,
            };
        }
    }
}
