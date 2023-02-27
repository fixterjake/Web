using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ZME.API.Data;
using ZME.API.Services;
using ZME.API.Shared.Models;
using ZME.API.Shared.Utils;

namespace ZME.API.Repositories;

public class AirportRepository
{
    private readonly DatabaseContext _context;
    private readonly LoggingService _loggingService;

    public AirportRepository(DatabaseContext context, LoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    public async Task<Airport> CreateAirport(Airport data, HttpRequest request)
    {
        var result = await _context.Airports.AddAsync(data);
        await _context.SaveChangesAsync();

        var newData = JsonConvert.SerializeObject(result.Entity);
        await _loggingService.AddWebsiteLog(request, $"Created airport '{result.Entity.Id}'", string.Empty, newData);

        return result.Entity;
    }

    public async Task<IList<Airport>> GetAirports()
    {
        return await _context.Airports.ToListAsync();
    }

    public async Task<Airport> GetAirport(int airportId)
    {
        return await _context.Airports.FindAsync(airportId) ??
            throw new AirportNotFoundException($"Airport '{airportId}' not found");
    }

    public async Task<Airport> UpdateAirport(Airport data, HttpRequest request)
    {
        var airport = await _context.Airports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.Id) ??
            throw new AirportNotFoundException($"Airport '{data.Id}' not found");

        var oldData = JsonConvert.SerializeObject(airport);

        data.Updated = DateTimeOffset.UtcNow;

        var result = _context.Airports.Update(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated airport '{data.Id}'", oldData, newData);

        return result.Entity;
    }

    public async Task DeleteAirport(int airportId, HttpRequest request)
    {
        var airport = await GetAirport(airportId);

        var oldData = JsonConvert.SerializeObject(airport);
        _context.Airports.Remove(airport);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted airport '{airportId}'", oldData, string.Empty);
    }
}