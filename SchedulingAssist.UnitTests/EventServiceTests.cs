using Microsoft.Extensions.Logging;
using NSubstitute;
using SchedulingAssist.Application.Common.Interfaces;
using SchedulingAssist.Application.Events;
using SchedulingAssist.Application.RequestModels;
using SchedulingAssist.Domain.Events;

namespace SchedulingAssist.UnitTests;

[TestFixture]
public class EventServiceTests
{
    private IEventRepository _eventRepository = null!;
    private EventService _eventService = null!;

    [SetUp]
    public void SetUp()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        var logger = Substitute.For<ILogger<EventService>>();
        _eventService = new EventService(_eventRepository, logger);
    }

    [Test]
    public async Task Given_ValidEventDetails_When_CreateEventAsync_Then_ReturnsEventIdAndPersistsEvent()
    {
        // Arrange
        const long expectedEventId = 101;
        const long userId = 7;
        var startTime = new DateTimeOffset(2026, 8, 18, 9, 0, 0, TimeSpan.Zero);
        var endTime = startTime.AddHours(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        _eventRepository.AddAsync(Arg.Any<Event>(), cancellationToken).Returns(Task.FromResult(expectedEventId));

        // Act
        var result = await _eventService.CreateEvent(
            "  Consultation  ", "  Annual check-up  ", startTime, endTime, userId, cancellationToken);

        // Assert
        Assert.That(result, Is.EqualTo(expectedEventId));
        await _eventRepository.Received(1).AddAsync(
            Arg.Is<Event>(e =>
                e.Title == "Consultation" &&
                e.Description == "Annual check-up" &&
                e.StartTime == startTime &&
                e.EndTime == endTime &&
                e.CreatedByUserId == userId),
            cancellationToken);
    }

    [Test]
    public void Given_EndTimeBeforeStartTime_When_CreateEventAsync_Then_ThrowsArgumentExceptionAndDoesNotPersistEvent()
    {
        // Arrange
        var startTime = new DateTimeOffset(2026, 8, 18, 10, 0, 0, TimeSpan.Zero);
        var endTime = startTime.AddMinutes(-30);

        // Act
        var action = () => _eventService.CreateEvent(
            "Consultation", "Annual check-up", startTime, endTime, 7, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await action());
        _eventRepository.DidNotReceive().AddAsync(Arg.Any<Event>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Given_ExistingEvent_When_UpdateAsync_Then_UpdatesEventAndPersistsIt()
    {
        // Arrange
        const long eventId = 12;
        const long userId = 7;
        var existingEvent = CreateEvent(eventId);
        var request = new UpdateEventRequest(
            "Follow-up consultation", "Review treatment plan",
            new DateTime(2026, 8, 19, 13, 0, 0), new DateTime(2026, 8, 19, 14, 0, 0));
        _eventRepository.GetByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Event?>(existingEvent));

        // Act
        await _eventService.UpdateAsync(eventId, request, userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(existingEvent.Title, Is.EqualTo(request.Title));
            Assert.That(existingEvent.Description, Is.EqualTo(request.Description));
            Assert.That(existingEvent.StartTime, Is.EqualTo(request.StartTime));
            Assert.That(existingEvent.EndTime, Is.EqualTo(request.EndTime));
        });
        await _eventRepository.Received(1).UpdateAsync(existingEvent, userId, Arg.Any<CancellationToken>());
    }

    [Test]
    public void Given_MissingEvent_When_UpdateAsync_Then_ThrowsKeyNotFoundExceptionAndDoesNotPersistEvent()
    {
        // Arrange
        const long eventId = 12;
        var request = new UpdateEventRequest("Follow-up", "Review", DateTime.Today, DateTime.Today.AddHours(1));
        _eventRepository.GetByIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Event?>(null));

        // Act
        var action = () => _eventService.UpdateAsync(eventId, request, 7);

        // Assert
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await action());
        Assert.That(exception!.Message, Does.Contain(eventId.ToString()));
        _eventRepository.DidNotReceive().UpdateAsync(Arg.Any<Event>(), Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Given_ActiveEvent_When_CancelAsync_Then_CancelsEventAndPersistsIt()
    {
        // Arrange
        const long eventId = 12;
        const long userId = 7;
        var existingEvent = CreateEvent(eventId);
        _eventRepository.GetByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Event?>(existingEvent));

        // Act
        await _eventService.CancelAsync(eventId, userId);

        // Assert
        Assert.That(existingEvent.IsCancelled, Is.True);
        await _eventRepository.Received(1).UpdateAsync(existingEvent, userId, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Given_ActiveEvent_When_DeleteAsync_Then_DeletesEventAndPersistsIt()
    {
        // Arrange
        const long eventId = 12;
        const long userId = 7;
        var existingEvent = CreateEvent(eventId);
        _eventRepository.GetByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Event?>(existingEvent));

        // Act
        await _eventService.DeleteAsync(eventId, userId);

        // Assert
        Assert.That(existingEvent.IsDeleted, Is.True);
        await _eventRepository.Received(1).UpdateAsync(existingEvent, userId, Arg.Any<CancellationToken>());
    }

    [Test]
    public void Given_MissingEvent_When_CancelAsync_Then_ThrowsKeyNotFoundExceptionAndDoesNotPersistEvent()
    {
        // Arrange
        const long eventId = 12;
        _eventRepository.GetByIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Event?>(null));

        // Act
        var action = () => _eventService.CancelAsync(eventId, 7);

        // Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await action());
        _eventRepository.DidNotReceive().UpdateAsync(Arg.Any<Event>(), Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    private static Event CreateEvent(long eventId) =>
        Event.Rehydrate(
            eventId, "Consultation", "Annual check-up",
            new DateTimeOffset(2026, 8, 18, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 18, 10, 0, 0, TimeSpan.Zero), 7);
}
