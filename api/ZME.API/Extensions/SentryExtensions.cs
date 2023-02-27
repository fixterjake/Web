using Microsoft.AspNetCore.Mvc;
using Sentry;
using ZME.API.Shared.Responses;

namespace ZME.API.Extensions;

public static class SentryExtensions
{
    public static ActionResult ReturnActionResult(this SentryId id)
    {
        return new BadRequestObjectResult(new Response<string?>
        {
            StatusCode = 500,
            Message = "An error has occurred",
            Data = id.ToString()
        });
    }
}