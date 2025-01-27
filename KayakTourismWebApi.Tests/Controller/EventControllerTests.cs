using FakeItEasy;
using FluentAssertions;
using KayakData.DTOs.EventNS;
using KayakData.InterfacesNS;
using KayakData.ModelsNS;
using KayakTourismWebApi.ControllersNS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KayakTourismWebApi.Tests.Controller
{
    public class EventControllerTests
    {
        private readonly EventsController _eventController;
        private readonly IEventRepository _eventRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventControllerTests()
        {
            // Dependencies
            _eventRepository = A.Fake<IEventRepository>();
            _httpContextAccessor = A.Fake<HttpContextAccessor>();

            // SUT
            _eventController = new EventsController(_eventRepository);
        }

        [Fact]
        public void EventsController_GetAll_ReturnsSuccess()
        {
            // Arrange
            var events = A.Fake<List<Event>>();
            var queryObj = A.Fake<QueryObject>();
            A.CallTo(() => _eventRepository.GetAllAsync(queryObj)).Returns(events);

            // Act
            var result = _eventController.GetAll(queryObj);

            // Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }

        [Fact]
        public void EventsController_GetAll_ShouldNotBeNull()
        {
            // Arrange
            var events = A.Fake<List<Event>>();
            var queryObj = A.Fake<QueryObject>();
            A.CallTo(() => _eventRepository.GetAllAsync(queryObj)).Returns(events);

            // Act

            var responseResult = _eventController.GetAll(queryObj);

            // Assert
            responseResult.Should().NotBeNull();
        }

        [Fact]
        public async Task EventsController_GetAll_ReturnsOk()
        {
            // Arrange
            var events = A.Fake<List<Event>>();
            var queryObj = A.Fake<QueryObject>();
            A.CallTo(() => _eventRepository.GetAllAsync(queryObj)).Returns(events);

            // Act
            var responseResult = await _eventController.GetAll(queryObj);

            // Assert
            responseResult.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task EventsController_CreateEvent_ReturnsCreatedAtAction()
        {
            // Arrange
            var newEvent = A.Fake<Event>();
            var newEventDto = A.Fake<CreateEventDto>();
            A.CallTo(() => _eventRepository.CreateEventAsync(newEvent)).Returns(newEvent);

            // Act
            var responseResult = await _eventController.CreateEvent(newEventDto);

            // Assert
            responseResult.Should().BeOfType(typeof(CreatedAtActionResult));
        }

        [Fact]
        public async Task EventsController_CreateEvent_ShouldNotBeNull()
        {
            // Arrange
            var newEvent = A.Fake<Event>();
            var newEventDto = A.Fake<CreateEventDto>();
            A.CallTo(() => _eventRepository.CreateEventAsync(newEvent)).Returns(newEvent);

            // Act
            var responseResult = await _eventController.CreateEvent(newEventDto);

            // Assert
            responseResult.Should().NotBeNull();
        }

        [Fact]
        public async Task EventsController_GetById_ReturnsNotFoundWhenEventDoesntExist()
        {
            // Arrange
            A.CallTo(() => _eventRepository.GetByIdAsync(A<int>.Ignored)).Returns(Task.FromResult<Event?>(null));

            // Act
            var responseResult = await _eventController.GetById(1);

            // Assert
            responseResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EventsController_GetbyId_ReturnsOk()
        {
            // Arrange
            A.CallTo(() => _eventRepository.GetByIdAsync(42)).Returns(new Event());
            
            // Act
            var responseResult = await _eventController.GetById(42);

            // Assert
            responseResult.Should().NotBeNull();
            responseResult.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task EventsController_GetById_ReturnsRightValue()
        {
            // Arrange
            var testEvent = new Event
            { 
                Id = 1, 
                Name = "Test Event", 
                Description = "Test Description", 
                Price = 100, 
                EventStarts = DateTime.Now, 
                EventEnds = DateTime.Now.AddDays(2) 
            };

            A.CallTo(() => _eventRepository.GetByIdAsync(1)).Returns(testEvent);

            // Act
            var result = await _eventController.GetById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject; 
            var eventDto = okResult.Value.Should().BeOfType<EventDto>().Subject; 
            eventDto.Id.Should().Be(testEvent.Id); 
            eventDto.Name.Should().Be(testEvent.Name); 
            eventDto.Description.Should().Be(testEvent.Description); 
            eventDto.Price.Should().Be(testEvent.Price); 
            eventDto.EventStarts.Should().Be(testEvent.EventStarts);
            eventDto.EventEnds.Should().Be(testEvent.EventEnds);
        }

        [Fact]
        public async Task EventsController_DeleteEvent_ReturnsNotFoundWhenEventDoesNotExist()
        {
            // Arrange
            A.CallTo(() => _eventRepository.DeleteEventAsync(1)).Returns(Task.FromResult<Event?>(null));

            // Act
            var result = await _eventController.DeleteEventAsync(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EventsController_DeleteEvent_ReturnsNoContentWhenDeletedSuccessfuly()
        {
            // Arrange
            var testEvent = new Event 
            { 
                Id = 1, 
                Name = "Test Event", 
                Description = "Test Description", 
                Price = 100, 
                EventStarts = DateTime.Now, 
                EventEnds = DateTime.Now.AddDays(2) 
            };

            A.CallTo(() => _eventRepository.DeleteEventAsync(1)).Returns(testEvent);

            // Act
            var result = await _eventController.DeleteEventAsync(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task EventsController_UpdateEvent_ReturnsOk()
        {
            // Arrange
            
            A.CallTo(() => _eventRepository.UpdateEventAsync(1, A.Fake<UpdateEventDto>())).Returns(A.Fake<Event>());

            // Act
            var responseResult = await _eventController.UpdateEvent(1, A.Fake<UpdateEventDto>());

            // Assert
            responseResult.Should().NotBeNull();
            responseResult.Should().BeOfType(typeof(OkObjectResult));
        }
        [Fact]
        public async Task EventsController_UpdateEvent_ReturnsRightValue()
        {
            // Arrange
            var testUpdateEventDto = new UpdateEventDto
            {
                Name = "Updated test Event",
                Description = "updated test Description",
                Price = 101,
                EventStarts = DateTime.Now,
                EventEnds = DateTime.Now.AddDays(2)
            };

            var testEvent = new Event
            {
                Name = "Updated test Event",
                Description = "updated test Description",
                Price = 101,
                EventStarts = DateTime.Now,
                EventEnds = DateTime.Now.AddDays(2)
            };

            A.CallTo(() => _eventRepository.UpdateEventAsync(1, testUpdateEventDto)).Returns(testEvent);

            // Act
            var responseResult = await _eventController.UpdateEvent(1, testUpdateEventDto);

            // Assert
            var okResult = responseResult.Should().BeOfType<OkObjectResult>().Subject;
            var eventDto = okResult.Value.Should().BeOfType<EventDto>().Subject;
            eventDto.Name.Should().Be(testEvent.Name);
            eventDto.Description.Should().Be(testEvent.Description);
            eventDto.Price.Should().Be(testEvent.Price);
            eventDto.EventStarts.Should().Be(testEvent.EventStarts);
            eventDto.EventEnds.Should().Be(testEvent.EventEnds);
        }
        [Fact]
        public async Task EventsController_UpdateEvent_ReturnsNotFoundWhenEventDoesNotExist()
        {
            // Arrange
            A.CallTo(() => _eventRepository.UpdateEventAsync(A<int>.Ignored, A<UpdateEventDto>.Ignored)).Returns(Task.FromResult<Event?>(null));

            // Act
            var responseResult = await _eventController.UpdateEvent(1, A.Fake<UpdateEventDto>());

            // Assert
            responseResult.Should().BeOfType<NotFoundResult>();
        }
    }

}

    