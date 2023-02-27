using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VATSIM.Connect.AspNetCore.Server.Authentication;
using VATSIM.Connect.AspNetCore.Server.Controllers;
using VATSIM.Connect.AspNetCore.Server.Options;
using VATSIM.Connect.AspNetCore.Server.Services;

namespace ZME.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public class AuthController : AuthControllerBase
{
    public AuthController(IVatsimConnectService vatsimService, IJwtAuthManager jwtAuthManager,
        IVatsimAuthenticationService authenticationService, IOptions<VatsimServerOptions> vatsimServerOptions,
        ILogger<AuthController> logger) : base(vatsimService, jwtAuthManager, authenticationService,
        vatsimServerOptions, logger)
    {
    }
}