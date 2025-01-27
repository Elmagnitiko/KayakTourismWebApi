using FluentAssertions;
using KayakData.DatabaseServicesNS;
using KayakData.DataNS;
using KayakData.ModelsNS;
using Microsoft.EntityFrameworkCore;

namespace KayakTourismWebApi.Tests.Repository
{
    public class EventSubscriptionRepositoryTests
    {
        public async Task<ApplicationDBContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase("In memory events subscription repo")
                .Options;

            var databaseContext = new ApplicationDBContext(options);
            await databaseContext.Database.EnsureCreatedAsync();
            return databaseContext;
        }

        [Fact]
        public async Task CloseRegistration_ReturnsEventModel_WhenIsClosedSuccessfully()
        {
            // Arrange
            var theEvent = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 1,
            };
            var referenceEventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0
            };

            var dbContext =  await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);
            await dbContext.Events.AddAsync(theEvent);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.CloseRegistrationAsync(1);

            // Assert
            result.Should().NotBeNull()
                .And.BeEquivalentTo(referenceEventModel);

            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task CloseRegistration_ReturnsNull_WhenCanNotFindSuchAnEvent()
        {
            // Arrange
            var theEvent = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 1,
            };

            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);
            await dbContext.Events.AddAsync(theEvent);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.CloseRegistrationAsync(2);

            // Assert
            result.Should().BeNull();

            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task OpenRegistration_ReturnsEventModel_WhenIsOpenedSuccessfully()
        {
            // Arrange
            var theEvent = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };
            var referenceEventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 1
            };

            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);
            await dbContext.Events.AddAsync(theEvent);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.OpenRegistrationAsync(1);

            // Assert
            result.Should().NotBeNull()
                .And.BeEquivalentTo(referenceEventModel);
            await dbContext.Database.EnsureDeletedAsync();
        }
        [Fact]
        public async Task OpenRegistration_ReturnsNull_WhenCanNotFindSuchAnEvent()
        {
            // Arrange
            var theEvent = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };

            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);
            await dbContext.Events.AddAsync(theEvent);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.OpenRegistrationAsync(2);

            // Assert
            result.Should().BeNull();
            await dbContext.Database.EnsureDeletedAsync();
        }
        [Fact]
        public async Task GetById_ReturnsRequestedEvent_WhenAllDataValid()
        {
            // Arrange
            var eventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };
            var eventCustomerModel1 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "1"
            };
            var eventCustomerModel2 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "2"
            };

            var customerModel1 = new Customer
            {
                Id = "1",
                UserName = "test@example1.com",
                Email = "test@example1.com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };
            var customerModel2 = new Customer
            {
                Id = "2",
                UserName = "test@example2.com",
                Email = "test@example2com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };
            var referenceEventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
                EventCustomers =
                {
                    eventCustomerModel1,
                    eventCustomerModel2,
                }
            };
            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);

            await dbContext.Events.AddAsync(eventModel);
            await dbContext.Users.AddRangeAsync(customerModel1, customerModel2);
            await dbContext.EventCustomers.AddRangeAsync(eventCustomerModel1, eventCustomerModel2);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull()
                .And.BeEquivalentTo(referenceEventModel);
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenCanNotFindSuchAnEvent()
        {
            // Arrange
            var eventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };

            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);

            await dbContext.Events.AddAsync(eventModel);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.GetByIdAsync(42);

            // Assert
            result.Should().BeNull();
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task SubscribeToEvent_ReturnsEventModel_WhenSubscribedSuccessfully()
        {
            // Arrange
            var eventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };
            var eventCustomerModel1 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "1"
            };
            var eventCustomerModel2 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "2"
            };
            var customerModel1 = new Customer
            {
                Id = "1",
                UserName = "test@example1.com",
                Email = "test@example1.com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };
            var customerModel2 = new Customer
            {
                Id = "2",
                UserName = "test@example2.com",
                Email = "test@example2com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };
            var referenceEventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
                EventCustomers =
                {
                    eventCustomerModel1,
                    eventCustomerModel2,
                }
            };

            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);

            await dbContext.Events.AddAsync(eventModel);
            await dbContext.Users.AddRangeAsync(customerModel1, customerModel2);
            await dbContext.EventCustomers.AddAsync(eventCustomerModel1);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.SubscribeToEventAsync(eventCustomerModel2, 1);

            // Assert
            result.Should().NotBeNull()
                .And.BeEquivalentTo(referenceEventModel);
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task SubscribeToEvent_ReturnsNull_WhenCanNotFindSuchAnEvent()
        {
            // Arrange
            var eventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };
            var eventCustomerModel1 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "1"
            };
            var customerModel1 = new Customer
            {
                Id = "1",
                UserName = "test@example1.com",
                Email = "test@example1.com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };

            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);

            await dbContext.Events.AddAsync(eventModel);
            await dbContext.Users.AddAsync(customerModel1);
            await dbContext.EventCustomers.AddAsync(eventCustomerModel1);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.SubscribeToEventAsync(eventCustomerModel1, 42);

            // Assert
            result.Should().BeNull();
            await dbContext.Database.EnsureDeletedAsync();
        }
        [Fact]
        public async Task DeleteCustomerFromEvent_ReturnsEventCustomerModel_WhenDeletedSuccessfully()
        {
            // Arrange
            var eventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };
            var eventCustomerModel1 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "1"
            };
            var customerModel1 = new Customer
            {
                Id = "1",
                UserName = "test@example1.com",
                Email = "test@example1.com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };

            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);
            await dbContext.Events.AddAsync(eventModel);
            await dbContext.Users.AddAsync(customerModel1);
            await dbContext.EventCustomers.AddAsync(eventCustomerModel1);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.DeleteCustomerFromEvent(1, "1");

            // Assert
            result.Should().NotBeNull()
                .And.BeEquivalentTo(eventCustomerModel1);
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task DeleteCustomerFromEvent_ReturnsNull_WhenCanNotFindAnEvent()
        {
            // Arrange
            var eventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };
            var eventCustomerModel1 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "1"
            };
            var customerModel1 = new Customer
            {
                Id = "1",
                UserName = "test@example1.com",
                Email = "test@example1.com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };

            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);
            await dbContext.Events.AddAsync(eventModel);
            await dbContext.Users.AddAsync(customerModel1);
            await dbContext.EventCustomers.AddAsync(eventCustomerModel1);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.DeleteCustomerFromEvent(42, "1");

            // Assert
            result.Should().BeNull();
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetAllAppliedCustomers_ReturnsArrayOfCustomers_WhenAnyCustomerAppliedToEvent()
        {
            // Arrange
            var eventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };
            var eventCustomerModel1 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "1"
            };
            var eventCustomerModel2 = new EventCustomer
            {
                EventId = 1,
                CustomerId = "2"
            };

            var customerModel1 = new Customer
            {
                Id = "1",
                UserName = "test@example1.com",
                Email = "test@example1.com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };
            var customerModel2 = new Customer
            {
                Id = "2",
                UserName = "test@example2.com",
                Email = "test@example2com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };
            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);

            dbContext.Events.Add(eventModel);
            dbContext.Users.AddRange(customerModel1, customerModel2);
            dbContext.EventCustomers.AddRange(eventCustomerModel1, eventCustomerModel2);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.GetAllAppliedCustomersAsync(1);

            // Assert
            result.Should().NotBeNull()
                .And.BeEquivalentTo(new[]
                {
                    customerModel1,
                    customerModel2,
                });
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetAllAppliedCustomers_ReturnsEmptyArray_WhenNoCustomersAreApplied()
        {
            // Arrange
            var eventModel = new Event
            {
                Id = 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 0,
            };
            var eventCustomerModel1 = new EventCustomer
            {
                EventId = 3,
                CustomerId = "1"
            };
            var customerModel1 = new Customer
            {
                Id = "1",
                UserName = "test@example1.com",
                Email = "test@example1.com",
                PhoneNumber = "+375291234567",
                TwoFactorEnabled = false,
            };
            var dbContext = await GetDatabaseContext();
            var eventSubscriptionRepo = new EventSubscriptionRepository(dbContext);
            await dbContext.Events.AddAsync(eventModel);
            await dbContext.EventCustomers.AddAsync(eventCustomerModel1);
            await dbContext.Users.AddAsync(customerModel1);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await eventSubscriptionRepo.GetAllAppliedCustomersAsync(1);

            // Assert
            result.Should().NotBeNull().And.BeEmpty();

            await dbContext.Database.EnsureDeletedAsync();
        }

    }
}
