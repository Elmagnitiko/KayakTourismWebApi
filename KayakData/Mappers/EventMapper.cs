using KayakData.DTOs.EventNS;
using KayakData.ModelsNS;

namespace KayakData.MappersNS
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

        public static void ToEventFromUpdateDto(this UpdateEventDto updateEventDto, Event eventModel)
        {
            eventModel.Name = updateEventDto.Name;
            eventModel.Description = updateEventDto.Description;
            eventModel.Price = updateEventDto.Price;
            eventModel.EventEnds = updateEventDto.EventEnds;
            eventModel.EventStarts = updateEventDto.EventStarts;
        }

        public static EventDto ToEventDto(this Event eventModel)
        {
            return new EventDto
            {
                Id = eventModel.Id,
                Name = eventModel.Name,
                Description = eventModel.Description,
                Price = eventModel.Price,
                EventStarts = eventModel.EventStarts,
                EventEnds = eventModel.EventEnds,
            };
        }
    }
}
