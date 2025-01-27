using FluentAssertions;
using KayakData.DatabaseServicesNS;
using KayakData.DataNS;
using KayakData.DTOs.EventNS;
using KayakData.ModelsNS;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KayakTourismWebApi.Tests.Repository
{
    public class EventRepositoryTests
    {
        public async Task<ApplicationDBContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase("In memory events repo")
                .Options;

            var databaseContext = new ApplicationDBContext(options);
            await databaseContext.Database.EnsureCreatedAsync();

            if (await databaseContext.Events.CountAsync() <= 0)
            {
                for (var i = 0; i < 10; i++)
                {
                    await databaseContext.Events.AddAsync(
                        new Event
                        {
                            Id = i + 42,
                            Name = $"Test {i + 1}",
                            Description = $"Test description {i + 1}",
                            Price = 123,
                            EventStarts = new DateTime(2025, 4, i + 1),
                            EventEnds = new DateTime(2025, 4, i + 3),
                            IsRegistrationOpenedInt = 1,
                        });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async Task CreateEvent_ReturnsEventModel_WhenCreatedSuccessfully()
        {
            // Arrange
            var theEvent = new Event
            {
                Id= 1,
                Name = "Test",
                Description = "Test description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 1,
            };
            var dbContext = await GetDatabaseContext();
            var eventRepo = new EventRepository(dbContext);

            // Act
            var result = await eventRepo.CreateEventAsync(theEvent);

            // Assert
            result.Should().BeEquivalentTo(theEvent);

            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task DeleteEvent_ReturnsEventModel_WhenDeletedSuccessfully()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var eventModel = await dbContext.Events.FindAsync(43);
            var eventRepo = new EventRepository(dbContext);
            // Act
            var result = await eventRepo.DeleteEventAsync(43);

            // Assert
            result.Should().BeEquivalentTo(eventModel);

            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task DeleteEvent_ReturnsNull_WhenCannotFindSuchAnEvent()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var eventRepo = new EventRepository(dbContext);
            // Act
            var result = await eventRepo.DeleteEventAsync(-42);

            // Assert
            result.Should().BeNull();

            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetAllEvents_ReturnsListOfEvents_WhenDatabaseIsNotEmpty()
        {
            // Arange
            var dbContext = await GetDatabaseContext();
            var eventRepo = new EventRepository(dbContext);
            var referenceValues = await dbContext.Events.ToListAsync();

            // Act
            var result = await eventRepo.GetAllAsync(new QueryObject
            {
                PageNumber = 1,
                PageSize = 10,
            });

            // Assert
            result.Should().NotBeNull()
                .And.HaveCount(referenceValues.Count)
                .And.BeEquivalentTo(referenceValues);

            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetById_ReturnsRequestedEventModel_WhenAllOk()
        {
            // Arange
            var id = 43;
            var dbContext = await GetDatabaseContext();
            var referenceEventModel = await dbContext.Events.FindAsync(id);
            var eventRepo = new EventRepository(dbContext);

            // Act
            var result = await eventRepo.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull()
                .And.BeEquivalentTo(referenceEventModel);

            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenCanNotFindSuchAnEvent()
        {
            // Arange
            var id = -43;
            var dbContext = await GetDatabaseContext();
            var eventRepo = new EventRepository(dbContext);

            // Act
            var result = await eventRepo.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();

            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task UpdateEvent_ReturnsUpdatedEvent_WhenAllDataIsValid()
        {
            // Arrange
            var id = 42;
            var referenceEventModel = new Event
            {
                Id = id,
                Name = "Updated name",
                Description = "Updated description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
                IsRegistrationOpenedInt = 1,
            };
            var updateEventModel = new UpdateEventDto
            {
                Name = "Updated name",
                Description = "Updated description",
                Price = 123,
                EventStarts = new DateTime(2025, 4, 1),
                EventEnds = new DateTime(2025, 4, 3),
            };
            var dbContext = await GetDatabaseContext();
            var eventRepo = new EventRepository(dbContext);

            // Act
            var result = await eventRepo.UpdateEventAsync(id, updateEventModel);

            // Assert
            result.Should().NotBeNull()
                .And.BeEquivalentTo(referenceEventModel);

            await dbContext.Database.EnsureDeletedAsync();
        }
        [Fact]
        public async Task UpdateEvent_ReturnsNull_WhenCanNotFindSuchAnEvent()
        {
            // Arrange
            var id = -42;
            var updateEventModel = new UpdateEventDto();
            var dbContext = await GetDatabaseContext();
            var eventRepo = new EventRepository(dbContext);

            // Act
            var result = await eventRepo.UpdateEventAsync(id, updateEventModel);

            // Assert
            result.Should().BeNull();

            await dbContext.Database.EnsureDeletedAsync();
        }
    }
}
