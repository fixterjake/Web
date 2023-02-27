using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VATSIM.Connect.AspNetCore.Server.Services;
using VATSIM.Connect.AspNetCore.Shared.DTO;
using ZME.API.Data;
using ZME.API.Shared.Enums;

namespace ZME.API.Services;

public class AuthenticationService : IVatsimAuthenticationService
{

    private readonly DatabaseContext _context;
    private readonly RedisService _redisService;

    public AuthenticationService(DatabaseContext context, RedisService redisService)
    {
        _context = context;
        _redisService = redisService;
    }

    public async Task<IEnumerable<Claim>> ProcessVatsimUserLogin(VatsimUserDto user)
    {
        var claims = new List<Claim>();
        var u = await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == user.Cid);

        claims.Add(new Claim("RatingInt", $"{user.VatsimDetails.ControllerRating.Id}"));

        if (u == null || u.Status == UserStatus.REMOVED)
        {
            claims.Add(new Claim("IsMember", $"{false}"));
            claims.Add(new Claim("roles", string.Empty));
            return claims;
        }

        claims.Add(new Claim("IsMember", $"{true}"));

        if (u.Roles == null)
        {
            claims.Add(new Claim("roles", string.Empty));
            return claims;
        }

        var roles = u.Roles.Select(x => x.NameShort).ToList();
        if (u.CanRegisterForEvents)
            roles.Add("CanRegisterForTraining");
        if (u.CanRequestTraining)
            roles.Add("CanRequestTraining");
        claims.AddRange(roles.Select(x => new Claim("roles", x)));

        await _redisService.SetRoles(roles, u.Id);

        return claims;
    }
}

