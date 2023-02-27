using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ZME.API.Data;
using ZME.API.Extensions;
using ZME.API.Services;
using ZME.API.Shared.Models;
using ZME.API.Shared.Utils;

namespace ZME.API.Repositories;

public class CommentRepository
{
    private readonly DatabaseContext _context;
    private readonly LoggingService _loggingService;

    public CommentRepository(DatabaseContext context, LoggingService loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    public async Task<Comment> CreateComment(Comment data, HttpRequest request)
    {
        if (!await _context.Users.AnyAsync(x => x.Id == data.UserId))
            throw new UserNotFoundException($"User '{data.UserId}' not found");
        data.SubmitterId = request.HttpContext.GetCid() ??
            throw new UserNotFoundException("Submitter id not found");

        var result = await _context.Comments.AddAsync(data);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created comment '{result.Entity.Id}'", string.Empty, newData);

        return result.Entity;
    }

    public async Task<IList<Comment>> GetComments(int userId, int page, int size, bool confidential)
    {
        if (!await _context.Users.AnyAsync(x => x.Id == userId))
            throw new UserNotFoundException($"User '{userId}' not found");
        if (confidential)
            return await _context.Comments
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Timestamp)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync();
        return await _context.Comments
            .Where(x => x.UserId == userId)
            .Where(x => !x.Confidential)
            .OrderBy(x => x.Timestamp)
            .Skip((page - 1) * size).Take(size)
            .ToListAsync();
    }

    public async Task<Comment> GetComment(int commentId)
    {
        return await _context.Comments.FindAsync(commentId) ??
            throw new CommentNotFoundException($"Comment '{commentId}' not found");
    }

    public async Task<int> GetCommentCount(int userId)
    {
        return await _context.Comments.Where(x => x.User.Id == userId).CountAsync();
    }

    public async Task DeleteComment(int commentId, HttpRequest request)
    {
        var comment = await GetComment(commentId);

        var oldData = JsonConvert.SerializeObject(comment);
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted comment '{commentId}'", oldData, string.Empty);
    }
}