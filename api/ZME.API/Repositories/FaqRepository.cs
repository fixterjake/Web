using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ZME.API.Data;
using ZME.API.Services;
using ZME.API.Shared.Models;
using ZME.API.Shared.Utils;

namespace ZME.API.Repositories;

public class FaqRepository
{
    private readonly DatabaseContext _context;
    private readonly LoggingService _loggingService;

    public FaqRepository(DatabaseContext context, LoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    public async Task<Faq> CreateFaq(Faq data, HttpRequest request)
    {
        var result = await _context.Faq.AddAsync(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created faq '{result.Entity.Id}'", string.Empty, newData);

        return result.Entity;
    }

    public async Task<IList<Faq>> GetFaqs()
    {
        return await _context.Faq.OrderBy(x => x.Order).ToListAsync();
    }

    public async Task<Faq> GetFaq(int faqId)
    {
        return await _context.Faq.FindAsync(faqId) ??
            throw new FaqNotFoundException($"Faq '{faqId}' not found");
    }

    public async Task<Faq> UpdateFaq(Faq data, HttpRequest request)
    {
        var faq = await _context.Faq.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.Id) ??
            throw new FaqNotFoundException($"Faq '{data.Id}' not found");

        var oldData = JsonConvert.SerializeObject(faq);

        data.Updated = DateTimeOffset.UtcNow;

        var result = _context.Faq.Update(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated faq '{data.Id}'", oldData, newData);

        return result.Entity;
    }

    public async Task DeleteFaq(int faqId, HttpRequest request)
    {
        var faq = await GetFaq(faqId);

        var oldData = JsonConvert.SerializeObject(faq);
        _context.Faq.Remove(faq);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted faq '{faqId}'", oldData, string.Empty);
    }
}
