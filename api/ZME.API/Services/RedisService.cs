﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Security.Claims;
using ZME.API.Data;

namespace ZME.API.Services;

public class RedisService
{
    private readonly DatabaseContext _context;
    private readonly IDatabase _redis;
    private readonly ILogger<RedisService> _logger;

    public RedisService(DatabaseContext context, IDatabase redis, ILogger<RedisService> logger)
    {
        _context = context;
        _redis = redis;
        _logger = logger;
    }

    public async Task<IList<string>?> GetRoles(ClaimsPrincipal user)
    {
        var cidRaw = user.Claims.FirstOrDefault(x => x.Type == "cid");
        if (cidRaw == null || !int.TryParse(cidRaw.Value, out var cid))
        {
            _logger.LogInformation("[RedisService] Cid was null or an invalid int: {Cid}", cidRaw?.Value);
            return null;
        }

        var rolesRaw = await _redis.StringGetAsync($"roles-{cid}");
        if (rolesRaw.IsNull)
        {
            var dbUser = await _context.Users
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == cid);
            if (dbUser == null)
                return new List<string>();

            _logger.LogInformation("[RedisService] roles not in redis, adding to redis and returning roles");

            var roles = dbUser.Roles.Select(x => x.NameShort).ToList();
            if (dbUser.CanRegisterForEvents)
                roles.Add("CanRegisterForEvents");
            if (dbUser.CanRequestTraining)
                roles.Add("CanRequestTraining");
            await SetRoles(roles, dbUser.Id);
            return roles;
        }

        _logger.LogInformation("[RedisService] Roles in redis, returning them");

        return JsonConvert.DeserializeObject<IList<string>>(rolesRaw!);
    }

    public async Task SetRoles(IList<string> roles, int userId)
    {
        await _redis.StringSetAsync($"roles-{userId}", JsonConvert.SerializeObject(roles), TimeSpan.FromMinutes(15));
    }

    public async Task<bool> ValidateRoles(ClaimsPrincipal user, string[] roles)
    {
        var userRoles = await GetRoles(user);
        if (userRoles == null)
            return false;
        foreach (var entry in userRoles)
            if (roles.Any(x => x == entry))
                return true;
        return false;
    }

    public async Task RefreshRoles(int userId)
    {
        var dbUser = await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (dbUser == null)
            return;

        var roles = dbUser.Roles.Select(x => x.NameShort).ToList();
        if (dbUser.CanRegisterForEvents)
            roles.Add("CanRegisterForEvents");
        if (dbUser.CanRequestTraining)
            roles.Add("CanRequestTraining");
        await SetRoles(roles, dbUser.Id);
    }
}
