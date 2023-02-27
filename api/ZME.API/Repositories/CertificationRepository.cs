using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ZME.API.Data;
using ZME.API.Services;
using ZME.API.Shared.Models;
using ZME.API.Shared.Utils;

namespace ZME.API.Repositories;

public class CertificationRepository
{
    private readonly DatabaseContext _context;
    private readonly LoggingService _loggingService;

    public CertificationRepository(DatabaseContext context, LoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    public async Task<Certification> CreateCertification(Certification data, HttpRequest request)
    {
        var result = await _context.Certifications.AddAsync(data);
        await _context.SaveChangesAsync();

        var newData = JsonConvert.SerializeObject(result.Entity);
        await _loggingService.AddWebsiteLog(request, $"Created certification '{result.Entity.Id}'", string.Empty, newData);

        return result.Entity;
    }

    public async Task<IList<Certification>> GetCertifications()
    {
        return await _context.Certifications.ToListAsync();
    }

    public async Task<Certification> GetCertification(int certificationId)
    {
        return await _context.Certifications.FindAsync(certificationId) ??
            throw new CertificationNotFoundException($"Certification '{certificationId}' not found");
    }

    public async Task<Certification> UpdateCertification(Certification data, HttpRequest request)
    {
        var certification = await _context.Certifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.Id) ??
            throw new CertificationNotFoundException($"Certification '{data.Id}' not found");

        var oldData = JsonConvert.SerializeObject(certification);

        data.Updated = DateTimeOffset.UtcNow;

        var result = _context.Certifications.Update(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated certification '{result.Entity.Id}'", oldData, newData);

        return result.Entity;
    }

    public async Task DeleteCertification(int certificationId, HttpRequest request)
    {
        var certification = await GetCertification(certificationId);

        // Delete user certifications first
        var userCertifications = await _context.UserCertifications.Where(x => x.CertificationId == certificationId).ToListAsync();
        var oldData = JsonConvert.SerializeObject(userCertifications);
        _context.UserCertifications.RemoveRange(userCertifications);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted user certifications with certification id '{certificationId}'", oldData, string.Empty);

        // Now delete the certification
        _context.Certifications.Remove(certification);
        await _context.SaveChangesAsync();
    }
}