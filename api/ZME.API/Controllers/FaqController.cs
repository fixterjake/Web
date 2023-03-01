using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sentry;
using ZME.API.Extensions;
using ZME.API.Repositories;
using ZME.API.Services;
using ZME.API.Shared.Models;
using ZME.API.Shared.Responses;
using ZME.API.Shared.Utils;
using Constants = ZME.API.Shared.Utils.Constants;

namespace ZME.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public class FaqController : ControllerBase
{
    private readonly FaqRepository _faqRepository;
    private readonly RedisService _redisService;
    private readonly IValidator<Faq> _faqValidator;
    private readonly IHub _sentryHub;

    public FaqController(FaqRepository faqRepository, RedisService redisService, IValidator<Faq> faqValidator, IHub sentryHub)
    {
        _faqRepository = faqRepository;
        _redisService = redisService;
        _faqValidator = faqValidator;
        _sentryHub = sentryHub;
    }

    [HttpPost]
    [Authorize(Roles = Constants.CAN_FAQ)]
    [ProducesResponseType(typeof(Response<Faq>), 201)]
    [ProducesResponseType(typeof(Response<IList<ValidationFailure>>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Faq>>> CreateFaq(Faq data)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_FAQ_LIST))
                return StatusCode(401);

            var validation = await _faqValidator.ValidateAsync(data);
            if (!validation.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = 400,
                    Message = "Validation failure",
                    Data = validation.Errors
                });
            }

            var result = await _faqRepository.CreateFaq(data, Request);
            await _redisService.RemoveCached("faqs");
            return StatusCode(201, new Response<Faq>
            {
                StatusCode = 201,
                Message = $"Created faq '{result.Id}'",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(Response<Faq>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Faq>>> GetFaq(int faqId)
    {
        try
        {
            var result = await _faqRepository.GetFaq(faqId);
            return Ok(new Response<Faq>
            {
                StatusCode = 200,
                Message = $"Got faq '{faqId}'",
                Data = result
            });
        }
        catch (FaqNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("All")]
    [ProducesResponseType(typeof(Response<IList<Faq>>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Faq>>> GetFaqs()
    {
        try
        {
            var cached = await _redisService.GetCached("faqs");
            if (cached != null)
            {
                var cachedResult = JsonConvert.DeserializeObject<IList<Faq>>(cached);
                return Ok(new Response<IList<Faq>>
                {
                    StatusCode = 200,
                    Message = $"Got {cachedResult?.Count} faqs",
                    Data = cachedResult
                });
            }
            var result = await _faqRepository.GetFaqs();
            await _redisService.SetCached("faqs", JsonConvert.SerializeObject(result));
            return Ok(new Response<IList<Faq>>
            {
                StatusCode = 200,
                Message = $"Got {result.Count} faqs",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut]
    [Authorize(Roles = Constants.CAN_FAQ)]
    [ProducesResponseType(typeof(Response<Faq>), 200)]
    [ProducesResponseType(typeof(Response<IList<ValidationFailure>>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Faq>>> UpdateFaq(Faq data)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_FAQ_LIST))
                return StatusCode(401);

            var validation = await _faqValidator.ValidateAsync(data);
            if (!validation.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = 400,
                    Message = "Validation failure",
                    Data = validation.Errors
                });
            }

            var result = await _faqRepository.UpdateFaq(data, Request);
            await _redisService.RemoveCached("faqs");
            return StatusCode(200, new Response<Faq>
            {
                StatusCode = 200,
                Message = $"Updated faq '{result.Id}'",
                Data = result
            });
        }
        catch (FaqNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpDelete]
    [Authorize(Roles = Constants.CAN_FAQ)]
    [ProducesResponseType(typeof(Response<string?>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<string?>>> DeleteFaq(int faqId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_FAQ_LIST))
                return StatusCode(401);

            await _faqRepository.DeleteFaq(faqId, Request);
            await _redisService.RemoveCached("faqs");
            return Ok(new Response<string?>
            {
                StatusCode = 200,
                Message = $"Deleted faq '{faqId}'"
            });
        }
        catch (FaqNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }
}
