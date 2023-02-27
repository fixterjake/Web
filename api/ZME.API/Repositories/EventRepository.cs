using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ZME.API.Data;
using ZME.API.Extensions;
using ZME.API.Services;
using ZME.API.Shared.Enums;
using ZME.API.Shared.Models;
using ZME.API.Shared.Utils;

namespace ZME.API.Repositories;

public class EventRepository
{
    private readonly DatabaseContext _context;
    private readonly LoggingService _loggingService;
    private readonly S3Service _s3Service;

    public EventRepository(DatabaseContext context, LoggingService loggingService, S3Service s3Service)
    {
        _context = context;
        _loggingService = loggingService;
        _s3Service = s3Service;
    }

    public async Task<Event> CreateEvent(Event data, HttpRequest request)
    {
        data.BannerUrl = await _s3Service.UploadFile(request, "events");

        if (data.Start <= DateTimeOffset.UtcNow)
            throw new InvalidEventException("Start time must be in the future");
        if (data.Start > data.End)
            throw new InvalidEventException("Start time must be before end time");
        if (data.End <= DateTimeOffset.UtcNow)
            throw new InvalidEventException("End time must be in the future");

        var result = await _context.Events.AddAsync(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created event '{result.Entity.Id}'", string.Empty, newData);

        return result.Entity;
    }

    public async Task<IList<Event>> GetEvents(int page, int size, bool getClosed)
    {
        return getClosed ?
            await _context.Events
                .OrderBy(x => x.Start)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync() :
            await _context.Events
                .Where(x => x.IsOpen)
                .OrderBy(x => x.Start)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync();
    }

    public async Task<Event> GetEvent(int eventId, bool getClosed)
    {
        var result = await _context.Events.FindAsync(eventId) ??
            throw new EventNotFoundException($"Event '{eventId}' not found");
        if (!getClosed && !result.IsOpen)
            throw new EventNotFoundException($"Event '{eventId}' not found");
        return result;
    }

    public async Task<int> GetEventsCount(bool getClosed)
    {

        return getClosed ?
            await _context.Events.CountAsync() :
            await _context.Events.Where(x => x.IsOpen).CountAsync();
    }

    public async Task<Event> UpdateEvent(Event data, HttpRequest request)
    {
        var @event = await _context.Events.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.Id) ??
            throw new EventNotFoundException($"Event '{data.Id}' not found");

        if (@event.BannerUrl != null)
            await _s3Service.DeleteFile(@event.BannerUrl);

        data.BannerUrl = await _s3Service.UploadFile(request, "events");
        data.Updated = DateTimeOffset.UtcNow;

        var oldData = JsonConvert.SerializeObject(data);
        var result = _context.Events.Update(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated event '{result.Entity.Id}'", oldData, newData);

        return result.Entity;
    }

    public async Task DeleteEvent(int eventId, HttpRequest request)
    {
        var dbEvent = await _context.Events.FindAsync(eventId) ??
            throw new EventNotFoundException($"Event '{eventId}' not found");

        if (dbEvent.BannerUrl != null)
            await _s3Service.DeleteFile(dbEvent.BannerUrl);

        var oldData = JsonConvert.SerializeObject(dbEvent);
        _context.Events.Remove(dbEvent);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted event '{eventId}'", oldData, string.Empty);
    }

    public async Task<EventPosition> CreateEventPosition(EventPosition data, HttpRequest request)
    {
        if (!await _context.Events.AnyAsync(x => x.Id == data.EventId))
            throw new EventNotFoundException($"Event '{data.EventId}' not found");

        var result = await _context.EventPositions.AddAsync(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created event position '{result.Entity.Id}'", string.Empty, newData);

        return result.Entity;
    }

    public async Task<IList<EventPosition>> GetEventPositions(int eventId, bool getClosed)
    {
        var dbEvent = await GetEvent(eventId, getClosed);
        return await _context.EventPositions.Where(x => x.EventId == eventId).ToListAsync();
    }

    public async Task DeleteEventPosition(int eventPositionId, HttpRequest request)
    {
        var dbEventPosition = await _context.EventPositions.FindAsync(eventPositionId) ??
            throw new EventPositionNotFoundException($"Event position '{eventPositionId}' not found");
        var dbRegistrations = await _context.EventRegistrations.Where(x => x.EventPositionId == eventPositionId).ToListAsync();

        // Delete registrations that are for the given position
        foreach (var entry in dbRegistrations)
        {
            var registrationOldData = JsonConvert.SerializeObject(entry);

            // todo: send email that position was removed
            _context.EventRegistrations.Remove(entry);
            await _context.SaveChangesAsync();
            await _loggingService.AddWebsiteLog(request, $"Deleted event position '{entry.Id}'", registrationOldData, string.Empty);
        }

        // Now delete the position
        var oldData = JsonConvert.SerializeObject(dbEventPosition);
        _context.EventPositions.Remove(dbEventPosition);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted event position '{eventPositionId}'", oldData, string.Empty);
    }

    public async Task<EventRegistration> CreateEventRegistration(EventRegistration data, HttpRequest request)
    {
        var @event = await _context.Events.FindAsync(data.EventId) ??
            throw new EventNotFoundException($"Event '{data.EventId}' not found");
        var position = await _context.EventPositions.FindAsync(data.EventPositionId) ??
            throw new EventPositionNotFoundException($"Event position '{data.EventPositionId}' not found");
        var user = await request.HttpContext.GetUser(_context) ??
            throw new UserNotFoundException("User not found");

        var existingRegistrations = await _context.EventRegistrations
            .AnyAsync(x => x.EventId == data.EventId && x.UserId == user.Id);

        var validationFailures = new List<ValidationFailure>();
        if (existingRegistrations)
        {
            validationFailures.Add(new ValidationFailure
            {
                PropertyName = nameof(data.EventId),
                AttemptedValue = data.EventId,
                ErrorMessage = $"User already has an event registration for event '{data.EventId}'",
            });
            throw new InvalidEventRegistrationException(JsonConvert.SerializeObject(validationFailures));
        }
        if (user.Rating < position.MinRating)
            validationFailures.Add(new ValidationFailure
            {
                PropertyName = nameof(user.Rating),
                AttemptedValue = user.Rating,
                ErrorMessage = $"User rating is less than {position.MinRating}",
            });
        if (data.Start < @event.Start.AddMinutes(-1))
            validationFailures.Add(new ValidationFailure
            {
                PropertyName = nameof(data.Start),
                AttemptedValue = data.Start,
                ErrorMessage = $"Registration start '{data.Start:u}' is invalid, must be after event start '{@event.Start:u}'",
            });
        if (data.Start > @event.End.AddMinutes(1))
            validationFailures.Add(new ValidationFailure
            {
                PropertyName = nameof(data.Start),
                AttemptedValue = data.Start,
                ErrorMessage = $"Registration start '{data.Start:u}' is invalid, must be before event end '{@event.End:u}'",
            });
        if (data.End < @event.Start.AddMinutes(-1))
            validationFailures.Add(new ValidationFailure
            {
                PropertyName = nameof(data.End),
                AttemptedValue = data.End,
                ErrorMessage = $"Registration end '{data.End:u}' is invalid, must be after event start '{@event.Start:u}'",
            });
        if (data.End > @event.End.AddMinutes(1))
            validationFailures.Add(new ValidationFailure
            {
                PropertyName = nameof(data.End),
                AttemptedValue = data.End,
                ErrorMessage = $"Registration start '{data.End:u}' is invalid, must be before event end '{@event.End:u}'",
            });

        if (validationFailures.Any())
            throw new InvalidEventRegistrationException(JsonConvert.SerializeObject(validationFailures));

        data.UserId = user.Id;

        var result = await _context.EventRegistrations.AddAsync(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        // todo: send confirmation email

        await _loggingService.AddWebsiteLog(request, $"Created event registration '{result.Entity.Id}'", string.Empty, newData);

        return result.Entity;
    }

    public async Task<EventRegistration> GetOwnEventRegistration(int eventId, HttpRequest request)
    {
        if (!await _context.Events.AnyAsync(x => x.Id == eventId))
            throw new EventNotFoundException($"Event '{eventId}' not found");
        var user = await request.HttpContext.GetUser(_context) ??
            throw new UserNotFoundException("User not found");

        return await _context.EventRegistrations.FirstOrDefaultAsync(x => x.EventId == eventId && x.UserId == user.Id) ??
            throw new EventRegistrationNotFoundException("Event registration not found");
    }

    public async Task<IList<EventRegistration>> GetEventRegistrations(int eventId)
    {
        if (!await _context.Events.AnyAsync(x => x.Id == eventId))
            throw new EventNotFoundException($"Event '{eventId}' not found");

        var positionEventIds = await _context.EventPositions
            .Where(x => x.EventId == eventId)
            .Select(x => x.Id)
            .ToListAsync();

        return await _context.EventRegistrations
            .Where(x => positionEventIds.Contains(x.EventPositionId))
            .ToListAsync();
    }

    public async Task<EventRegistration> AssignEventRegistration(int eventRegistrationId, HttpRequest request)
    {
        var eventRegistration = await _context.EventRegistrations.FindAsync(eventRegistrationId) ??
            throw new EventRegistrationNotFoundException($"Event registration '{eventRegistrationId}' not found");
        var eventPosition = await _context.EventPositions.FindAsync(eventRegistration.EventPositionId) ??
            throw new EventPositionNotFoundException($"Event position '{eventRegistration.EventPositionId}' not found");

        var oldData = JsonConvert.SerializeObject(eventRegistration);
        eventRegistration.Status = EventRegistrationStatus.ASSIGNED;
        eventRegistration.Updated = DateTimeOffset.UtcNow;
        eventPosition.Available = false;
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(eventRegistration);

        // todo: send email

        await _loggingService.AddWebsiteLog(request,
            $"Assigned event registration '{eventRegistrationId}' to event position '{eventPosition.Id}'", oldData, newData);

        return eventRegistration;
    }

    public async Task<IList<EventRegistration>> AssignReliefEventRegistrations(int eventId, HttpRequest request)
    {
        if (!await _context.Events.AnyAsync(x => x.Id == eventId))
            throw new EventNotFoundException($"Event '{eventId}' not found");
        var positions = await _context.EventPositions.Where(x => x.EventId == eventId).ToListAsync();
        var unassignedRegistrations = await _context.EventRegistrations
            .Where(x => positions.Select(x => x.Id).Contains(x.EventPositionId))
            .Where(x => x.Status == EventRegistrationStatus.PENDING)
            .ToListAsync();

        foreach (var entry in unassignedRegistrations)
        {
            var oldData = JsonConvert.SerializeObject(entry);
            entry.Status = EventRegistrationStatus.RELIEF;
            entry.Updated = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            var newData = JsonConvert.SerializeObject(entry);

            await _loggingService.AddWebsiteLog(request, $"Assigned event registration '{entry.Id}' as relief", oldData, newData);

            // todo: send email
        }

        return unassignedRegistrations;
    }

    public async Task DeleteOwnEventRegistration(int eventId, HttpRequest request)
    {
        if (!await _context.Events.AnyAsync(x => x.Id == eventId))
            throw new EventNotFoundException($"Event '{eventId}' not found");
        var registration = await GetOwnEventRegistration(eventId, request);
        var position = await _context.EventPositions.FindAsync(registration.EventPositionId) ??
            throw new EventPositionNotFoundException("Event position not found");

        var oldData = JsonConvert.SerializeObject(registration);
        _context.EventRegistrations.Remove(registration);
        await _context.SaveChangesAsync();

        position.Available = true;
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"User deleted event registration '{registration.Id}'", oldData, string.Empty);

        // todo: send confirmation email

    }

    public async Task DeleteEventRegistration(int eventRegistrationId, HttpRequest request)
    {
        var registration = await _context.EventRegistrations.FindAsync(eventRegistrationId) ??
            throw new EventRegistrationNotFoundException($"Event registration '{eventRegistrationId}' not found");
        var position = await _context.EventPositions.FindAsync(registration.EventPositionId) ??
            throw new EventPositionNotFoundException("Event position not found");

        var oldData = JsonConvert.SerializeObject(registration);
        _context.EventRegistrations.Remove(registration);
        await _context.SaveChangesAsync();

        position.Available = true;
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted event registration '{registration.Id}'", oldData, string.Empty);

        // todo: send email that event staff delete registration
    }
}
