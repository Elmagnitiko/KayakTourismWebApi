using Castle.Core.Resource;
using FakeItEasy;
using FluentAssertions;
using KayakData.InterfacesNS;
using KayakData.ModelsNS;
using KayakTourismWebApi.ControllersNS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace KayakTourismWebApi.Tests.Controller
{
    public class EventSubscriptionControllerTests
    {
        private readonly IEventSubscription _eventSubscriptionRepo;
        private readonly UserManager<Customer> _userManager;
        private readonly EventSubscriptionController _eventSubscriptionController;
        private readonly HttpContext _httpContext;

        private readonly Customer fakeCustomer = new Customer
        {
            Id = "4",
            UserName = "test@example.com",
            Email = "test@example.com",
            PhoneNumber = "+375291234567",
            TwoFactorEnabled = false,
            EventCustomers = new List<EventCustomer>()
            {
                new EventCustomer()
                {
                    EventId = 1,
                    CustomerId = "4"
                }
            }
        };
        private readonly EventCustomer fakeEC = new EventCustomer
        {
            CustomerId = "4",
            EventId = 1,
        };
        private readonly Event fakeEvent = new Event
        {
            Id = 1,
            Name = "Test",
            Description = "Test description",
            Price = 123,
            EventCustomers = new List<EventCustomer>()
            {
                new EventCustomer()
                {
                    EventId = 1,
                    CustomerId = "4"
                }
            }
        };
        private readonly Event fakeEventWithClosedRegistration = new Event
        {
            Id = 2,
            Name = "Test",
            Description = "Test description",
            Price = 123,
            IsRegistrationOpened = false,
        };

        public EventSubscriptionControllerTests()
        {
            // Dependencies
            _eventSubscriptionRepo = A.Fake<IEventSubscription>();
            _userManager = A.Fake<UserManager<Customer>>();
            _httpContext = A.Fake<HttpContext>();

            // SUT
            _eventSubscriptionController = new EventSubscriptionController(_userManager, _eventSubscriptionRepo)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };
        }                                                                         

        [Fact]
        public async Task ApplyForEvent_ReturnsOk_WhenAllDataValid()
        {
            // Arrange
            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(fakeCustomer);

            A.CallTo(() => _eventSubscriptionRepo.GetByIdAsync(1))
                .Returns(fakeEvent);
            A.CallTo(() => _eventSubscriptionRepo.SubscribeToEventAsync(fakeEC, 1)).Returns(fakeEvent);

            // Act
            var result = await _eventSubscriptionController.ApplyForEvent(3);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("Successfully applied for the event.");
        }

        [Fact]
        public async Task ApplyForEvent_ReturnsUnthorized_WhenCanNotFindCustomer()
        {
            // Arrange
            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(Task.FromResult<Customer?>(null));

            // Act
            var result = await _eventSubscriptionController.ApplyForEvent(42);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>()
                .Which.Value.Should().Be("User not found.");
        }

        [Fact]
        public async Task ApplyForEvent_ReturnsNotFound_WhenCannotFindEvent()
        {
            // Arrange
            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(fakeCustomer);

            A.CallTo(() => _eventSubscriptionRepo.GetByIdAsync(A<int>.Ignored))
                .Returns(Task.FromResult<Event?>(null));

            // Act
            var result = await _eventSubscriptionController.ApplyForEvent(42);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("Event not found.");
        }

        [Fact]
        public async Task ApplyForEvent_ReturnsBadRequest_WhenCustomerIsAlereadySubscribedToTheEvent()
        {
            // Arrange
            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(fakeCustomer);

            A.CallTo(() => _eventSubscriptionRepo.GetByIdAsync(1))
                .Returns(fakeEvent);

            // Act
            var result = await _eventSubscriptionController.ApplyForEvent(1);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("User is already subscribed to this event.");
        }
        [Fact]
        public async Task ApplyForEvent_ReturnsBadRequest_WhenRegistrationClosed()
        {
            // Arrange
            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => _httpContext.User).Returns(fakeClaimsPrincipal);
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(fakeCustomer);

            A.CallTo(() => _eventSubscriptionRepo.GetByIdAsync(2))
                .Returns(fakeEventWithClosedRegistration);

            // Act
            var result = await _eventSubscriptionController.ApplyForEvent(2);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Registration for this event is closed.");
        }

    }
}
